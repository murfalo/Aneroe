using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

	public Tile otherTile;
	public Item[] usableItemTypes;

	public bool CanUseItem(Item item) {
		System.Type itemType = item.GetType ();
		for (int i = 0; i < usableItemTypes.Length; i++) {
			if (usableItemTypes.GetType().Equals(itemType)) {
				return true;
			}
		}
		return false;
	}

	// Use item on tile
	public virtual void UseItem(Item item) {}

	// Other tile calls this to affect it with item used on other tile
	public virtual void IndirectUseItem(Item item) {}

	void Start () {}

	void Update () {}
}
