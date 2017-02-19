using System;
using System.Collections.Generic;
using AneroeInputs;
using UIEvents;
using UnityEngine;

/// <summary>
///     This class manages the inventory UI as well as a simple event system
///     to allow easy inventory updating.
/// </summary>
public class InventoryController : BaseController
{
    /// <summary>Prefab for an inventory slot.</summary>
    [SerializeField] private GameObject UISlot;

    /// <summary>Prefab for an inventory item slot.</summary>
    [SerializeField] private GameObject InvSlot;

    /// <summary>Prefab for a physical (non-UI) item.</summary>
    [SerializeField] private GameObject Item;

    /// <summary>Number of items in a row of the inventory.</summary>
    public static int ItemsPerRow = 7;

    private List<GameObject> _slots;

    /// <summary>Event for an item moving in the inventory.</summary>
    public static event EventHandler<ItemMovedEventArgs> ItemMoved;

    /// <summary>Event for an item being equipped by the player.</summary>
    public static event EventHandler<EventArgs> ItemEquipped;

    /// <summary>Parent index of the newly selected item.</summary>
    private int _newParentIndex;

    /// <summary>Parent index of the previously selected item.</summary>
    private int _oldParentIndex;

    /// <summary>Initializes the inventory to the size of the currently active character.</summary>
    public override void ExternalSetup()
    {
        InputController.iEvent.inputed += ReceiveInput;
        SceneController.timeSwapped += RefreshInventory;
        SceneController.timeSwapped += RebindListener;
        SaveController.playerLoaded += RefreshInventory;
        UIController.ItemSelected += SelectItem;
        UIController.ItemSelected += MoveItem;

        _slots = new List<GameObject>();
        //GameObject itemHolder = GameObject.Find ("Items");
        for (var i = 0; i < PlayerController.activeCharacter.inv.maxItems; i++)
        {
            var newSlot = Instantiate(UISlot);
            newSlot.name = "Slot." + i;
            newSlot.transform.SetParent(UIController.Inventory.transform.GetChild(((i / ItemsPerRow) == 0) ? 0 : 1).transform);
            _slots.Add(newSlot);
        }
    }

    public override void RemoveEventListeners()
    {
        SceneController.timeSwapped -= RefreshInventory;
        SceneController.timeSwapped -= RebindListener;
        SaveController.playerLoaded -= RefreshInventory;
        UIController.ItemSelected -= SelectItem;
        UIController.ItemSelected -= MoveItem;
    }

    public void PickupOrRemoveItem(object source, ItemInteractEventArgs e)
	{
		if (e.addedToInv) {
			var nextSlot = e.inventory.SlotOf (e.item);
			var uiItem = _slots [nextSlot];

			var invItem = Instantiate (InvSlot);
			invItem.transform.SetParent (uiItem.transform);
			invItem.GetComponent<InventorySlot> ().SetUnsetItem (e.item, nextSlot);
			OnItemMoved (invItem, -1, nextSlot);
		} else {
			Destroy(_slots[e.oldIndex].GetComponentInChildren<InventorySlot>().gameObject);	
		}
    }

    /// <summary>Stores state when an item is selected from a UI slot.</summary>
    private void SelectItem(object source, ItemSelectedEventArgs eventArgs)
    {
        if (eventArgs.newSelected == null || !eventArgs.newSelected.CompareTag("UIItem")) return;
        if (UIController.GetUISection(eventArgs.newSelected) != "Inventory")
        {
            _oldParentIndex = -1;
            _newParentIndex = -1;
        }
        else
        {
            _oldParentIndex = _newParentIndex;
            _newParentIndex = _slots.IndexOf(eventArgs.newSelected.transform.parent.gameObject);
            OnItemMoved(eventArgs.newSelected, _newParentIndex, -1);
        }
    }

    /// <summary>Delegates item movement information according to UI item selection information.</summary>
    private void MoveItem(object source, ItemSelectedEventArgs eventArgs)
    {
        if (eventArgs.oldSelected == null || eventArgs.newSelected == null) return;
        if (eventArgs.newSelected.CompareTag("UISlot"))
            OnItemMoved(eventArgs.oldSelected, _newParentIndex, _slots.IndexOf(eventArgs.newSelected.gameObject));
        else
        {
            OnItemMoved(eventArgs.oldSelected, _oldParentIndex, _newParentIndex);
            _newParentIndex = -1;
        }

    }

    /// <summary>Publishes the itemMoved event if an item has changed positions in the inventory.</summary>
    private void OnItemMoved(GameObject go, int prevSlot, int newSlot)
    {
        var item = go.GetComponent<InventorySlot>().GetItem();
        if (ItemMoved != null)
          ItemMoved(this, new ItemMovedEventArgs(item, prevSlot, newSlot));
    }

    /// <summary>Refreshes the inventory using the active player's inventory data.</summary>
    private void RefreshInventory<T>(object source, T eventArgs)
    {
        //if (!typeof(T).IsAssignableFrom(typeof(EventArgs)))
        //    return;

        var activeInv = PlayerController.activeCharacter.inv;
        var numSlots = activeInv.maxItems;

        for (var i = 0; i < numSlots; i++)
        {
			var item = activeInv.GetItem(i);
            var oldSlot = _slots[i].GetComponentInChildren<InventorySlot>();
            if (oldSlot != null)
                Destroy(oldSlot.gameObject);
            if (item == null)
                continue;
            var invSlot = Instantiate(InvSlot);
            invSlot.transform.SetParent(_slots[i].transform);
            invSlot.GetComponent<InventorySlot>().SetUnsetItem(item, i);
        }
    }

    private void RebindListener(object source, PlayerSwitchEventArgs e)
    {
        if (e.oldPlayer)
            e.oldPlayer.itemInteracted -= PickupOrRemoveItem;
        e.newPlayer.itemInteracted += PickupOrRemoveItem;
    }

    public void ReceiveInput(object source, InputEventArgs eventArgs)
    {
        if (!eventArgs.WasPressed("equip")) return;
        var newEquipped = eventArgs.GetTrigger("equip");
        if (newEquipped != "")
            PlayerController.activeCharacter.inv.itemSlotEquipped = int.Parse(newEquipped) - 1;
        if (ItemEquipped != null)
            ItemEquipped(this, new EventArgs());
    }
}