using System;
using AneroeInputs;
using PlayerEvents;
using UIEvents;
using UnityEngine;
using UnityEngine.UI;

public class UIController : BaseController
{
    /// <summary>The game object representing the user interface.</summary>
    [SerializeField] private GameObject UI;

    /// <summary>Tooltip object for displaying item names and descriptions.</summary>
    public static GameObject Tooltip;

    /// <summary>The game object representing the inventory.</summary>
    public static GameObject Inventory;

    /// <summary>The game object representing the main menu.</summary>
    public static GameObject MainMenu;

    /// <summary>The game object representing the crafting menu.</summary>
    public static GameObject Crafting;

    /// <summary>Item currently selected by the player.</summary>
    public static GameObject Selected;

    /// <summary>Item currently selected by the player.</summary>
    [SerializeField] public static GameObject PlayerStatus;

    /// <summary>Event published when an item selected in the UI.</summary>
    public static event EventHandler<ItemSelectedEventArgs> ItemSelected;

    private GameObject _activeMenu;

    /// <summary>The text representing the player's health.</summary>
    private string _healthText
    {
        get { return PlayerStatus.transform.GetChild(1).GetComponent<Text>().text; }
        set { PlayerStatus.transform.GetChild(1).GetComponent<Text>().text = value; }
    }

    /// <summary>Causes the selected item to follow the mouse cursor.</summary>
    public void Update()
    {
        if (Selected)
            Selected.transform.position = Input.mousePosition;
    }

    /// <summary>Selects an item from or drops an item into a UI slot on left click.</summary>
    /// <param name="target">The target of the pointer click.</param>
    public void HandlePointerClick(GameObject target)
    {
        if (Selected)
            MoveItem(target, GetUISection(target) == "Inventory");
        else
            SelectItem(target, true);
    }

    /// <summary>Gets the UI section associated with the target object.</summary>
    /// <param name="target">Object to find the UI section for.</param>
    public static string GetUISection(GameObject target)
    {
        if (target == null) return "";
        var t = target.transform;
        while (t.transform.parent != null && t.transform.parent.name != "UI")
            t = t.transform.parent;
        return t.name;
    }

    /// <summary>Selects the target item from a UI slot.</summary>
    /// <param name="target">Either a UI item or UI slot to select an item from.</param>
    /// <param name="signal">Whether or not to publish ItemSelected.</param>
    private void SelectItem(GameObject target, bool signal)
    {
        if (target.CompareTag("UISlot")) Selected = null;
        if (!target.CompareTag("UIItem")) return;
        Selected = target;
        if (ItemSelected != null && signal)
            ItemSelected(this, new ItemSelectedEventArgs(null, Selected));
        Selected.transform.SetParent(Selected.GetComponentInParent<Canvas>().transform);
        target.GetComponent<Image>().raycastTarget = false;
    }

    /// <summary>Drops the selected item from the inventory.</summary>
    /// <param name="item">The item to drop from the inventory.</param>
    public static void DropItem(GameObject item)
    {
        if (item == null) return;
        var activeCharacter = PlayerController.activeCharacter;
        var newItem = item.GetComponent<InventorySlot>().GetItem();
        //newItem.transform.SetParent(GameObject.Find("Items").transform);
        newItem.DropItem(activeCharacter.GetInteractPosition());
        Selected = null;
        Destroy(item);
    }

    /// <summary>Moves the selected item in a UI slot.</summary>
    /// <param name="target">Either a UI item or a UI slot to move the currently selected item into.</param>
    private void MoveItem(GameObject target, bool signal)
    {
        if (GetUISection(target) == "Canvas")
        {
            DropItem(Selected);
        }
        else if (target.CompareTag("UIItem") || target.CompareTag("UISlot"))
        {
            if (target.CompareTag("UISlot") && target.transform.childCount > 0) return;
            Selected.GetComponent<Image>().raycastTarget = true;
            var newParent = target.CompareTag("UIItem") ? target.transform.parent : target.transform;
            Selected.transform.SetParent(newParent);
            if (ItemSelected != null && signal)
                ItemSelected(this, new ItemSelectedEventArgs(Selected, target));
            SelectItem(target, false);
        }
    }

    private void OnHealthChanged<T>(object source, T eventArgs)
    {
        if (typeof(T) == typeof(PlayerHealthChangedEventArgs))
            _healthText = ((PlayerHealthChangedEventArgs) (object) eventArgs).NewHealth.ToString();
        else if (typeof(T) == typeof(PlayerSwitchEventArgs))
            _healthText = ((PlayerSwitchEventArgs) (object) eventArgs).newPlayer.stats.GetStat("health").ToString();
    }

    /// <summary>Load in UI game objects.</summary>
    public override void InternalSetup()
    {
        for (var i = 0; i < UI.transform.childCount; i++)
        {
            var t = UI.transform.GetChild(i);
            switch (t.name)
            {
                case "Inventory":
                    Inventory = t.gameObject;
                    break;
                case "MainMenu":
                    MainMenu = t.gameObject;
                    break;
                case "Crafting":
                    Crafting = t.gameObject;
                    break;
                case "PlayerStatus":
                    PlayerStatus = t.gameObject;
                    break;
            }
        }
		Tooltip = Instantiate(Resources.Load<GameObject>("Prefabs/UI/Tooltip"));
        Tooltip.transform.SetParent(UI.transform);
        Tooltip.SetActive(false);
    }

    public override void ExternalSetup()
    {
        InputController.iEvent.inputed += ReceiveInput;
        PlayerController.PlayerHealthChanged += OnHealthChanged;
        SceneController.timeSwapped += OnHealthChanged;
    }

    public override void RemoveEventListeners()
    {
        InputController.iEvent.inputed -= ReceiveInput;
        PlayerController.PlayerHealthChanged -= OnHealthChanged;
        SceneController.timeSwapped -= OnHealthChanged;
    }

    public void ReceiveInput(object source, InputEventArgs eventArgs)
    {
        if (eventArgs.WasPressed("inventory"))
        {
            if (_activeMenu == null || _activeMenu == Inventory)
                ToggleInventory();
        }
        else if (eventArgs.WasPressed("mainmenu"))
        {
            if (PromptController.activePrompt != null)
            {
                PromptController.activePrompt.ContinuePrompt();
            }
            else
            {
                // If possible, deactivate other menu instead of activate main menu
                if (_activeMenu != null && _activeMenu != MainMenu)
                    ToggleInventory();
                else
                    MainMenu.SetActive(!MainMenu.activeSelf);
                // Update active menu
                _activeMenu = MainMenu.activeSelf ? MainMenu : null;
            }
        }

        InputController.mode = _activeMenu ? InputInfo.InputMode.UI : InputInfo.InputMode.Free;
    }

    private void ToggleInventory()
    {
        var hotbar = Inventory.transform.GetChild(0).gameObject.transform;
        foreach (Transform child in hotbar)
        {
            if (child.childCount == 0) continue;
            var image = child.GetChild(0).GetComponent<Image>();
            image.raycastTarget = !image.raycastTarget;
        }
        var extra = Inventory.transform.GetChild(1).gameObject;
        extra.SetActive(!extra.activeSelf);
        Crafting.SetActive(!Crafting.activeSelf);
        DropItem(Selected);
        GameObject.Find("Control").GetComponent<CraftingController>().DropItems();
        _activeMenu = _activeMenu != null ? null : Inventory;
    }
}