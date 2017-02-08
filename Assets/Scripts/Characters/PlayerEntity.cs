using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : Entity {

	public Inventory inv;

	public override void Setup() {
		base.Setup ();
		controller = GameObject.Find("Control").GetComponent<EntityController> ();

		inv = new Inventory();

		// Subscribe to the inventory controller events to handle UI events appropriately.
		InventoryController.itemMoved += OnItemMoved;
	}

	/// <summary>Event handler for the itemMoved event provided by ItemController.</summary>
	/// <param name="source">Originator of itemMoved event.</param>
	/// <param name="eventArgs">Useful context of the itemMoved event.</param>
	public void OnItemMoved(object source, InventoryEvents.ItemMovedEventArgs eventArgs)
	{
		inv.RemoveItem(eventArgs.prevSlot);
		if (eventArgs.newSlot != null)
			inv.SetItem((int)eventArgs.newSlot, eventArgs.item);
	}

	public void TryInteracting() {
		if (!CanActOutOfMovement())
			return;
		// right now, it only tries to place first item into ItemMound below
		Item i = inv.GetItem(0);
		Collider2D coll = Physics2D.OverlapCircle (transform.position, 0.01f, 1 << LayerMask.NameToLayer ("ItemMound"));
		if (i != null && coll != null && coll.gameObject.GetComponent<ItemMound> ().CanUseItem (i)) {
			coll.gameObject.GetComponent<ItemMound> ().UseItem (i);
			inv.RemoveItem (0);
			print("Putting Item into mound");
		} else if (coll != null) {
			inv.AddItem (coll.gameObject.GetComponent<ItemMound> ().RetrieveItem (this));
			print ("Taking Item out of mound");
		} else {
			print ("Miss");
		}
	}

	public void OnTriggerEnter2D (Collider2D coll) {
		// Attempting to pick up items off the ground
		Item i = coll.gameObject.GetComponent<Item> ();
		if (i != null && i.GetEntity() == null) {
			print (i.transform.parent.name);
			inv.AddItem (i);
			i.gameObject.GetComponent<Renderer> ().enabled = false;
			i.gameObject.GetComponent<Collider2D> ().enabled = false;
		}

	}
}
