using UnityEngine;
using System.Linq;
using System.Collections;

public class InteractivePlant : TileInteractive
{
	const string GROWTH_ITEM = "WaterPotion";
	// In addition to default full tile sprite
	public Sprite budTileSprite;

	public GameObject[] drops;
	Vector3 dropBounds;
	public bool stillGrowing;

	new void Start () {
		base.Start();
		usableItemTypes = new[] {typeof(Weapon)};
		usableItemPrefabNames = new[] { GROWTH_ITEM };

		dropBounds = coll.bounds.extents;
	}

	public override void UseItem (Item item, out Item newItem)
	{
		newItem = item;
		if (item.prefabName.Equals (GROWTH_ITEM)) {
			if (stillGrowing) {
				newItem = null;
				stillGrowing = false;
			}
		} else if (!stillGrowing) {
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
	}

	public override Hashtable Save() {
		Hashtable tsd = base.Save(); 
		tsd.Add ("growing", stillGrowing);
		return tsd;
	}

	public override void Load(Hashtable tsd) {
		base.Load (tsd);
		stillGrowing = (bool)tsd ["growing"];
		UpdateSprite ();

	}

	void UpdateSprite() {
		if (broken)
			sRend.sprite = brokenTileSprite;
		else if (stillGrowing) {
			sRend.sprite = budTileSprite;
		} else {
			sRend.sprite = fullTileSprite;
		}
	}
}

