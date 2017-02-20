using UnityEngine;
using System.Collections;

public class TileBreakable : Tile
{
	public Sprite fullTileSprite, brokenTileSprite;
	SpriteRenderer sRend;
	Collider2D coll;
	public bool isPast;

	// Use this for initialization
	void Start ()
	{
		sRend = GetComponent<SpriteRenderer> ();
		coll = GetComponent<Collider2D> ();
		usableItemTypes = new System.Type[1];
		usableItemTypes [0] = System.Type.GetType("Weapon");
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public override bool CanUseItem(Item item)
	{
	    interactState = base.CanUseItem(item) ? Entity.CharacterState.Interacting : default(Entity.CharacterState);
		return base.CanUseItem (item);
	}

	public override void UseItem (Item item, out Item newItem)
	{
		newItem = null;
		sRend.sprite = brokenTileSprite;
		coll.enabled = false;
		SendDisableTileEvent ();
		if (otherTile && isPast) {
            otherTile.IndirectUseItem(item, out newItem);
		}
	}
}

