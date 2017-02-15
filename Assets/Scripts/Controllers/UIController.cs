using System;
using AneroeInputs;
using UIEvents;
using UnityEngine;
using UnityEngine.UI;

public class UIController : BaseController
{
    /// <summary>The game object representing the user interface.</summary>
    [SerializeField] private GameObject UI;

    /// <summary>The game object representing the inventory.</summary>
    public static GameObject Inventory;

    /// <summary>The game object representing the main menu.</summary>
    public static GameObject MainMenu;

    /// <summary>The game object representing the crafting menu.</summary>
    public static GameObject Crafting;

    /// <summary>Item currently selected by the player.</summary>
    public static GameObject Selected;

    /// <summary>Event published when an item selected in the UI.</summary>
    public static event EventHandler<ItemSelectedEventArgs> ItemSelected;

    private GameObject _activeMenu;

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
            MoveItem(target);
        else
            SelectItem(target, true);
    }

    /// <summary>Gets the UI section associated with the target object.</summary>
    /// <param name="target">Object to find the UI section for.</param>
    private string GetUISection(GameObject target)
    {
        var t = target.transform;
        while (t.transform.parent != null && t.transform.parent.name != "UI")
            t = t.transform.parent;
        return t.name;
    }

    /// <summary>Selects the target item from a UI slot.</summary>
    /// <param name="target">Either a UI item or UI slot to select an item from.</param>
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
    private void MoveItem(GameObject target)
    {
        if (GetUISection(target) == "Canvas")
        {
            DropItem(Selected);
        }
        else if (target.CompareTag("UIItem") || target.CompareTag("UISlot"))
        {
            Selected.GetComponent<Image>().raycastTarget = true;
            var newParent = target.CompareTag("UIItem") ? target.transform.parent : target.transform;
            Selected.transform.SetParent(newParent);
            if (ItemSelected != null)
                ItemSelected(this, new ItemSelectedEventArgs(Selected, target));
            SelectItem(target, false);
        }
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
            }
        }
    }

    public override void ExternalSetup()
    {
        InputController.iEvent.inputed += ReceiveInput;
    }

    public override void RemoveEventListeners()
    {
        InputController.iEvent.inputed -= ReceiveInput;
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
            // If possible, deactivate other menu instead of activate main menu
            if (_activeMenu != null && _activeMenu != MainMenu) ToggleInventory();
            else MainMenu.SetActive(!MainMenu.activeSelf);
            // Update active menu
            _activeMenu = MainMenu.activeSelf ? MainMenu : null;
        }

        InputController.mode = _activeMenu ? InputInfo.InputMode.UI : InputInfo.InputMode.Free;
    }

    private void ToggleInventory()
    {
        Inventory.SetActive(!Inventory.activeSelf);
        Crafting.SetActive(!Crafting.activeSelf);
        DropItem(Selected);
        GameObject.Find("Control").GetComponent<CraftingController>().DropItems();
        _activeMenu = _activeMenu != null ? null : Inventory;
    }
}