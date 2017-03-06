using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AneroeInputs;
using UIEvents;

public class StorageController : BaseController
{
	protected List<GameObject> _slots;

	/// <summary>Parent index of the newly selected item.</summary>
	protected int _newParentIndex;

	/// <summary>Parent index of the previously selected item.</summary>
	protected int _oldParentIndex;

	/// <summary>Event for an item moving in the inventory.</summary>
	public static event EventHandler<ItemMovedEventArgs> ItemMoved;

	/// <summary>Initializes the inventory to the size of the currently active character.</summary>
	public override void ExternalSetup()
	{
		UIController.ItemSelected += SelectItem;
		UIController.ItemSelected += MoveItem;

		_slots = new List<GameObject>();
	}

	public override void RemoveEventListeners()
	{
		UIController.ItemSelected -= SelectItem;
		UIController.ItemSelected -= MoveItem;
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
				newSlot.transform.SetParent (UIController.Inventory.transform.GetChild (0).transform, false);
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
}

