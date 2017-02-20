using UnityEngine;
using System.Collections;

public class WaterTile : Tile
{
	public Sprite waterTile, iceTile;
	SpriteRenderer sRend;
	Collider2D coll;
	public bool isPast;

	// Use this for initialization
	void Start ()
	{
		sRend = GetComponent<SpriteRenderer> ();
		coll = GetComponent<Collider2D> ();
		usableItemTypes = new System.Type[1];
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

    public override bool CanUseItem(Item item)
    {
        interactState = base.CanUseItem(item) ? Entity.CharacterState.Interacting : default(Entity.CharacterState);
        return base.CanUseItem(item);
    }

    public override void UseItem (Item item, out Item newItem)
	{
		newItem = null;
		sRend.sprite = iceTile;
		coll.enabled = false;
		SendDisableTileEvent ();
		if (otherTile && isPast) {
            otherTile.IndirectUseItem(item, out newItem);
		}
	}

    public override void IndirectUseItem(Item item, out Item newItem)
    {
        newItem = null;
        if (isPast) return;
        sRend.sprite = iceTile;
        coll.enabled = false;
    }
}

