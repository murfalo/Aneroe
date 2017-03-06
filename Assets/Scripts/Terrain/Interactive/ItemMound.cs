using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Timelines;

public class ItemMound : TileInteractive {

	// Item held in tile
	List<Item> buriedItems;
	List<TimelinePeriod> itemPeriods;

	// Use this for initialization
	public override void Awake () {
		base.Awake ();
		primaryTile = !otherTile.primaryTile;
		sRend = GetComponent<SpriteRenderer> ();
		usableItemTypes = new Type[] {typeof(Item), typeof(Weapon)};
		usableItemPrefabNames = new[] {"Shovel"};
		interactState = Entity.CharacterState.Interacting;

		buriedItems = new List<Item>();
		itemPeriods = new List<TimelinePeriod> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public virtual bool CanUnburyItem(Item item) {
		return false;
	}
	/*
	// Bury item in mound
	public override void UseItem(Item item, out Item unburiedItem) {
		ItemMound otherMound = (ItemMound)otherTile;
		// passed item can be buried
		if (buriedItem == null && item != null && CanBuryItem(item)) 
		{
			if (isPast && otherMound.buriedItem != null) {
				// PARADOX: burying an item when an item is buried in the future in the same spot
				unburiedItem = item;
			} else {
				unburiedItem = null;
				if (isPast) {
					// That item must also appear in the future
					otherMound.IndirectUseItem (item);
				}
				UpdateBuriedItem (item);
			}
		}
		// buried item can be unburied
		else if (buriedItem != null) {
			if (boundForever) {
				// PARADOX: unburying an item you unburied in the future
				unburiedItem = item;
			} else {
				unburiedItem = buriedItem;
				// if past mound 
				if (isPast) {
					// That item must also disappear from the future
					otherMound.IndirectUseItem (null);
				} else if (otherMound.buriedItem != null) {
					// That item is what we're unburying, so it is now tied in the past mound
					otherMound.boundForever = true;
				}
				// Must be done after buriedItem's use above
				UpdateBuriedItem (null);
			}
		} else {
			unburiedItem = item;
		}
	}

	// Other tile calls this to affect it with item buried in other tile
	public void IndirectUseItem(Item item) {
		//print ("Indirect one: " + isPast + " item: " + item);
		UpdateBuriedItem (item);
	}

	void UpdateBuriedItem(Item item) {
		buriedItem = item;
		if (buriedItem != null)
			buriedItem.GiveItemTo (primaryTile ? transform : otherTile.transform);
		sRend.sprite = buriedItem != null ? fullTileSprite : emptyTileSprite;
	}

	public Item GetBuriedItem() {
		return buriedItem;
	}

	public override Hashtable Save() {
		Hashtable tsd = new Hashtable (); 
		if (buriedItem != null)
			tsd.Add ("item", buriedItem.Save ());
		else
			tsd.Add ("item", null);
		tsd.Add ("boundForever", boundForever);
		return tsd;
	}

	public override void Load(Hashtable tsd) {
		// Destroy old buried item(s, maybe later???)
		foreach (var i in GetComponentsInChildren<Item>())
			Destroy(i.gameObject);

		boundForever = (bool)tsd ["boundForever"];
		Hashtable itemSave = (Hashtable)tsd["item"];
		if (itemSave == null)
			return;
		Item item = ((ItemMound)otherTile).GetBuriedItem();
		if (primaryTile || item == null) {
			GameObject itemObj = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + (string)itemSave ["prefabName"]));
			itemObj.transform.SetParent (transform, false);
			item = itemObj.GetComponentInChildren<Item> ();
			item.Load (itemSave);
		}
		buriedItem = item;
		sRend.sprite = buriedItem != null ? fullTileSprite : emptyTileSprite;
	}
	*/
}
