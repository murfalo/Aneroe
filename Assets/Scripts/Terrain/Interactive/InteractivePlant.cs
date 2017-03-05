using UnityEngine;
using System.Linq;
using System.Collections;

public class InteractivePlant : Tile
{

	public Sprite fullTileSprite, brokenTileSprite;
	SpriteRenderer sRend;
	bool broken;
	Collider2D coll;
	public string[] usableItemPrefabNames;
	public GameObject drop;

	public override bool CanUseItem(Item item) {
		// If you're not wielding something, interaction is always allowed
		if (item != null && typeof(Weapon) == item.GetType ()) {
			Item i;
			this.UseItem (item,out i);
		}
		return false;
	}

	// Use this for initialization
	public void Start ()
	{
		usableItemPrefabNames = new string[0];
		usableItemTypes = new System.Type[0];
		sRend = GetComponent<SpriteRenderer> ();
		coll = GetComponent<Collider2D> ();
		broken = false;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public override void UseItem (Item item, out Item newItem)
	{
		newItem = null;
		if (typeof(Weapon) != item.GetType ()) {
			return;
		}

		sRend.enabled = false;
		broken = true;
		coll.enabled = false;
		GameObject dropItem = (GameObject)Instantiate (drop);
		Item i = dropItem.GetComponent<Item> ();
		i.Setup();
		i.DropItem(transform.position);
	}

	public override Hashtable Save() {
		Hashtable tsd = new Hashtable (); 
		tsd.Add ("broken", broken);
		tsd.Add ("col_enabled", coll.enabled);
		return tsd;
	}

	public override void Load(Hashtable tsd) {
		broken = (bool)tsd ["broken"];
		coll.enabled = (bool)tsd ["col_enabled"];
		sRend.enabled = broken ? false : true;
	}
}

