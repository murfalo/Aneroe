using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour {

	Image itemImage;
	Item item;

	void Awake() {
		itemImage = transform.GetComponent<Image> ();
	}

	public void SetUnsetItem(Item i) {
		item = i;
		bool enable = item != null;
		itemImage.enabled = enable;
		if (enable) {
			itemImage.sprite = item.GetSprite ();
		}
	}

	public Item GetItem() {
		return item;
	}
}
