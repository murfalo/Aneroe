using UnityEngine;
using System.Collections;

public class TileBreakable : Tile
{

	//NOTE: If othertile exists, it has to be a TileBreakable

	public Sprite fullTileSprite, brokenTileSprite;
	SpriteRenderer sRend;
	Collider2D coll;

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

	public override bool CanUseItem(Item item) {
		return base.CanUseItem (item);
	}

	public override bool UseItem (Item item)
	{
		if (this.CanUseItem (item)) {
			sRend.sprite = brokenTileSprite;
			coll.enabled = false;
			SendDisableTileEvent ();
			if (otherTile) {
				((TileBreakable)otherTile).sRend.sprite = ((TileBreakable)otherTile).brokenTileSprite;
				((TileBreakable)otherTile).coll.enabled = false;
				((TileBreakable)otherTile).SendDisableTileEvent ();
			}
			return true;
		}
		return false;
	}
}

