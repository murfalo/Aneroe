using UnityEngine;
using System;
using System.Linq;
using System.Collections;

public class ItemMound : Tile {

	// Item held in tile
	Item buriedItem;
	public bool isPast;
	bool boundForever;
	Type[] buriableItemTypes;

	// Player can interact with this tile when colliding with this
	Collider2D digRangeCollider;

	// Array of 2 sprites:
	// first sprite for when no item is buried 
	// second sprite for when an item is buried
	public Sprite emptyTileSprite, fullTileSprite;
	SpriteRenderer sRend;

	void Awake()
	{
		primaryTile = !otherTile.primaryTile;
	}

	// Use this for initialization
	void Start () {
		sRend = GetComponent<SpriteRenderer> ();
		buriedItem = null;
		buriableItemTypes = new Type[] { typeof(Item) };
		usableItemTypes = new Type[] {typeof(Item), typeof(Weapon)};
		interactState = Entity.CharacterState.Interacting;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public virtual bool CanBuryItem(Item item) {
		// If you're not wielding something, interaction is always allowed
		if (item == null)
			return true;
		var itemType = item.GetType ();
		return buriableItemTypes.Any(i => i == itemType);
	}

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
}
