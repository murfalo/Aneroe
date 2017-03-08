using UnityEngine;

public class InteractiveWater : TileInteractive {

	new void Start () {
		base.Start();
		usableItemPrefabNames = new[] {"IcePotion","PotionFlask"};
	}

	public override bool UseItem (Item item, out Item newItem)
	{
		if (item.prefabName.Equals ("IcePotion"))
			return base.UseItem (item, out newItem);
		GameObject itemObj = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Items/WaterPotion"));
		newItem = itemObj.GetComponent<Item> ();
		return true;
	}
}
