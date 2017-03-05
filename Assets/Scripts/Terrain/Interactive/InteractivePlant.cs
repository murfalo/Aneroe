using UnityEngine;
using System.Linq;
using System.Collections;

public class InteractivePlant : Tile
{

	public Sprite fullTileSprite, brokenTileSprite;
	SpriteRenderer sRend;
	bool broken;
	public bool isPast;
	public string[] usableItemPrefabNames;
	public GameObject drop;

	public override bool CanUseItem(Item item) {
		// If you're not wielding something, interaction is always allowed
		print ("Trying to Using Item");
		if (item == null) return true;
		var itemType = item.GetType();
		var canUse = itemType == System.Type.GetType("Item") ? usableItemPrefabNames.Any(n => n == item.prefabName) : usableItemTypes.Any(i => i == itemType);
		interactState = canUse ? Entity.CharacterState.Interacting : default(Entity.CharacterState);
		return canUse;
	}

	// Use this for initialization
	public void Start ()
	{
		usableItemPrefabNames = new string[0];
		usableItemTypes = new System.Type[0];
		sRend = GetComponent<SpriteRenderer> ();
		broken = false;
	}

	// Update is called once per frame
	void Update ()
	{
	}

	public override void UseItem (Item item, out Item newItem)
	{
		print ("Using Item");
		newItem = null;
		sRend.sprite = brokenTileSprite;
		broken = true;
		GameObject dropItem = (GameObject)Instantiate (drop);
		Item i = dropItem.GetComponent<Item> ();
		i.Setup();
		i.DropItem(transform.position);
	}

	public override Hashtable Save() {
		Hashtable tsd = new Hashtable (); 
		tsd.Add ("broken", broken);
		return tsd;
	}

	public override void Load(Hashtable tsd) {
		broken = (bool)tsd ["broken"];
		sRend.sprite = broken ? brokenTileSprite : fullTileSprite;
	}
}

