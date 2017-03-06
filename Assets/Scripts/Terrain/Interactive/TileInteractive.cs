using UnityEngine;
using System.Linq;
using System.Collections;

public class TileInteractive : Tile
{
	public Sprite fullTileSprite, brokenTileSprite;
	SpriteRenderer sRend;
	Collider2D coll;
	bool broken;
	public bool isPast;
	public string[] usableItemPrefabNames;

	public override bool CanUseItem(Item item) {
		// If you're not wielding something, interaction is always allowed
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
		sRend.sprite = brokenTileSprite;
		broken = true;
		coll.enabled = false;
		SendDisableTileEvent ();
		if (otherTile && isPast)
            otherTile.IndirectUseItem(item, out newItem);
	}

    public override void IndirectUseItem(Item item, out Item newItem)
    {
        newItem = typeof(Weapon) == item.GetType() ? item : null;
        if (isPast) return;
		coll.enabled = false;
        Debug.Log(coll);
		broken = true;
        sRend.sprite = brokenTileSprite;
    }

	public override Hashtable Save() {
	    var tsd = new Hashtable {{"broken", broken}, {"col_enabled", coll.enabled}};
	    return tsd;
	}

	public override void Load(Hashtable tsd) {
        Debug.Log(name + ' ' + coll.enabled);
        broken = (bool)tsd ["broken"];
        Debug.Log(coll);
		coll.enabled = (bool)tsd ["col_enabled"];
		sRend.sprite = broken ? brokenTileSprite : fullTileSprite;
	}
}

