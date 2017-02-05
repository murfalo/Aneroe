using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySelect : MonoBehaviour, IPointerClickHandler {

	InventoryController inventoryControl;

	void Start() {
		inventoryControl = GameObject.Find ("Control").GetComponent<InventoryController> ();
	}

	/// <section>Selects an item from or drops an item into a UI slot on left click.</section>
	/// <param name="eventData">Data about the pointer event.</param>
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		var target = eventData.pointerCurrentRaycast.gameObject;
		inventoryControl.HandlePointerClick (target);
	}
}
