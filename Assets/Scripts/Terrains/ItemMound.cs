using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMound : Tile {

	// Item held in tile
	Item buriedItem;

	// Player can interact with this tile when colliding with this
	Collider2D digRangeCollider;

	// Array of 2 sprites:
	// first sprite for when no item is buried 
	// second sprite for when an item is buried
	public Sprite emptyTileSprite, fullTileSprite;
	SpriteRenderer sRend;

	// Use this for initialization
	void Start () {
		sRend = GetComponent<SpriteRenderer> ();
		buriedItem = null;
		usableItemTypes = new System.Type[1];
		usableItemTypes [0] = System.Type.GetType("Herb");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override bool CanUseItem(Item i){
		return buriedItem == null;
	}

	// Bury item in mound
	public override bool UseItem(Item item) {
		if (this.CanUseItem(item)) {
			item.transform.SetParent(transform);
			buriedItem = item;
			sRend.sprite = fullTileSprite;
			((ItemMound) otherTile).buriedItem = item;
			Debug.Log (((ItemMound)otherTile).buriedItem);
			((ItemMound)otherTile).sRend.sprite = ((ItemMound)otherTile).fullTileSprite;
			return true;
		}
		return false; 
	}

	// Other tile calls this to affect it with item buried in other tile
	public override void IndirectUseItem(Item item) {}

	// Dig up item in mound
	public Item RetrieveItem() {
		sRend.sprite = emptyTileSprite;
		Item i = buriedItem;
		buriedItem = null;
		((ItemMound) otherTile).buriedItem = null;
		((ItemMound)otherTile).sRend.sprite = ((ItemMound)otherTile).emptyTileSprite;
		return i;
	}
}
