using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySelect : MonoBehaviour, IPointerClickHandler {

	UIController uiControl;

	void Start() {
		uiControl = GameObject.Find ("Control").GetComponent<UIController> ();
	}

	/// <section>Selects an item from or drops an item into a UI slot on left click.</section>
	/// <param name="eventData">Data about the pointer event.</param>
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button != PointerEventData.InputButton.Left) return;
		var target = eventData.pointerCurrentRaycast.gameObject;
		uiControl.HandlePointerClick (target);
	}
}
