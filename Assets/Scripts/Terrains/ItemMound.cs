using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMound : Tile {

	// Item held in tile
	Item buriedItem;

	// Player can interact with this tile when colliding with this
	Collider2D digRangeCollider;

	// Only applicable for ItemMounds in the past when the future pulls it out
	bool isHoldingItemForFuture;

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
		usableItemTypes [0] = System.Type.GetType("Item");
		isHoldingItemForFuture = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override bool CanUseItem(Item i){
		return buriedItem == null && base.CanUseItem(i) && !isHoldingItemForFuture;
	}

	// Bury item in mound
	public override bool UseItem(Item item) {
		if (this.CanUseItem(item)) {
			item.transform.SetParent(transform);
			buriedItem = item;
			sRend.sprite = fullTileSprite;
			if (otherTile && timeline == Timeline.Past) {
				((ItemMound)otherTile).buriedItem = item;
				((ItemMound)otherTile).sRend.sprite = ((ItemMound)otherTile).fullTileSprite;
			}
			return true;
		}
		return false; 
	}

	// Other tile calls this to affect it with item buried in other tile
	public override void IndirectUseItem(Item item) {}

	// Dig up item in mound
	public Item RetrieveItem() {
		if (!isHoldingItemForFuture) {
			sRend.sprite = emptyTileSprite;
			Item i = buriedItem;
			buriedItem = null;
			if (otherTile) {
				((ItemMound)otherTile).buriedItem = null;
				if (timeline == Timeline.Present) {
					((ItemMound)otherTile).isHoldingItemForFuture = true;
				} else {
					((ItemMound)otherTile).sRend.sprite = ((ItemMound)otherTile).emptyTileSprite;
				}
			}
			return i;
		} else {
			// Place for a text popup for the player
			Debug.Log ("This Tile is Holding an Item for the future");
		}
		return null;
	}
}
