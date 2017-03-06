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
	/// <summary>Overlay for selected item.</summary>
	[SerializeField] private GameObject selectedOverlay;

	/// <summary>Number of items in a row of the inventory.</summary>
	public static int ItemsPerRow = 7;

	private List<GameObject> _slots;

	/// <summary>Event for an item moving in the inventory.</summary>
	public static event EventHandler<ItemMovedEventArgs> ItemMoved;

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
		GameController.playerLoaded += RefreshInventory;
		UIController.ItemSelected += SelectItem;
		UIController.ItemSelected += MoveItem;

		_slots = new List<GameObject>();

	}

	public override void RemoveEventListeners()
	{
		SceneController.timeSwapped -= RefreshInventory;
		SceneController.timeSwapped -= RebindListener;
		GameController.playerLoaded -= RefreshInventory;
		UIController.ItemSelected -= SelectItem;
		UIController.ItemSelected -= MoveItem;
	}

	public void PickupOrRemoveItem(object source, ItemInteractEventArgs e)
	{
		if (e.addedToInv) {
			var nextSlot = e.inventory.SlotOf (e.item);
			var uiItem = _slots [nextSlot];

			var invItem = Instantiate (UIController.InvSlotPrefab);
			invItem.transform.SetParent (uiItem.transform, false);
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
		// Reinitialize slots if necessary
		if (_slots.Count < PlayerController.activeCharacter.inv.maxItems) {
			for (var i = _slots.Count; i < PlayerController.activeCharacter.inv.maxItems; i++) {
				var newSlot = Instantiate (UIController.UISlotPrefab);
				newSlot.name = "Slot." + i;
				newSlot.transform.SetParent (UIController.Inventory.transform.GetChild (((i / ItemsPerRow) == 0) ? 0 : 1).transform, false);
				_slots.Add (newSlot);
			}
		} else if (_slots.Count > PlayerController.activeCharacter.inv.maxItems) {
			int maxCount = PlayerController.activeCharacter.inv.maxItems;
			for (var i = maxCount; i < _slots.Count; i++) {
				_slots.RemoveAt (maxCount);
			}
		}

		// Load from active character inventory
		var activeInv = PlayerController.activeCharacter.inv;
		var numSlots = activeInv.maxItems;
		selectedOverlay.transform.position = _slots[PlayerController.activeCharacter.inv.itemSlotEquipped].transform.position;

		for (var i = 0; i < numSlots; i++)
		{
			var item = activeInv.GetItem(i);
			var oldSlot = _slots[i].GetComponentInChildren<InventorySlot>();
			if (oldSlot != null)
				Destroy(oldSlot.gameObject);
			if (item == null)
				continue;
			var invSlot = Instantiate(UIController.InvSlotPrefab);
			invSlot.transform.SetParent(_slots[i].transform, false);
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
		selectedOverlay.transform.position = _slots[PlayerController.activeCharacter.inv.itemSlotEquipped].transform.position;
	}
}