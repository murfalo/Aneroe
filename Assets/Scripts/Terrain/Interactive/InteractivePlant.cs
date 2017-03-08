using UnityEngine;
using System.Linq;
using System.Collections;

public class InteractivePlant : TileInteractive
{
	public enum GrowthStates {
		Planted,
		Growing,
		FullyGrown
	};

	const string GROWTH_ITEM = "WaterPotion";
	// In addition to default full tile sprite
	public Sprite budTileSprite, growingTileSprite;

	public GameObject[] drops;
	Vector3 dropBounds;
	GrowthStates growthState;

	new void Start () {
		base.Start();
		usableItemTypes = new[] {typeof(Weapon)};
		usableItemPrefabNames = new[] { GROWTH_ITEM };

		dropBounds = coll.bounds.extents;
		if (sRend.sprite.Equals (budTileSprite)) {
			growthState = GrowthStates.Planted;
		} else if (sRend.sprite.Equals (growingTileSprite)) {
			growthState = GrowthStates.Growing;
		} else {
			growthState = GrowthStates.FullyGrown;
		}
	}

	public void Plant() {
		growthState = GrowthStates.Planted;
		UpdateSprite ();
	}

	public override bool UseItem (Item item, out Item newItem)
	{
		newItem = item;
		if (item.prefabName.Equals (GROWTH_ITEM)) {
			if (growthState == GrowthStates.Growing) {
				newItem = null;
				growthState = GrowthStates.FullyGrown;
			}
		} else if (growthState == GrowthStates.FullyGrown) {
			broken = true;
			coll.enabled = false;
			foreach (GameObject drop in drops) {
				GameObject dropItem = (GameObject)Instantiate (drop);
				Item i = dropItem.GetComponent<Item> ();
				i.Setup ();
				i.DropItem (transform.position + new Vector3(Random.Range(-dropBounds.x, dropBounds.x),Random.Range(-dropBounds.y, dropBounds.y),0));
			}
		}
		UpdateSprite ();
		return true;
	}

	public override Hashtable Save() {
		Hashtable tsd = base.Save(); 
		tsd ["growing"] = growthState;
		return tsd;
	}

	public override void Load(Hashtable tsd) {
		base.Load (tsd);
		growthState = (GrowthStates)tsd ["growing"];
		UpdateSprite ();

	}

	void UpdateSprite() {
		if (broken) {
			sRend.sprite = brokenTileSprite;
			return;
		}
		switch (growthState) {
		case GrowthStates.Planted: 
			sRend.sprite = budTileSprite;
			break;
		case GrowthStates.Growing:
			sRend.sprite = growingTileSprite;
			break;
		case GrowthStates.FullyGrown:
			sRend.sprite = fullTileSprite;
			break;
		}
	}
}

