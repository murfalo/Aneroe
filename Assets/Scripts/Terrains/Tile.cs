using UnityEngine;
using System.Collections;
using TerrainEvents;

public class Tile : MonoBehaviour {

	public Tile otherTile;
	public System.Type[] usableItemTypes;
	public enum Timeline {
		Past,
		Present
	};

	public Timeline timeline;

	public virtual bool CanUseItem(Item item) {
		System.Type itemType = item.GetType ();
		for (int i = 0; i < usableItemTypes.Length; i++) {
			if (usableItemTypes[i].Equals(itemType)) {
				return true;
			}
		}
		return false;
	}

	// Use item on tile
	public virtual bool UseItem(Item item) {return false;}

	// Other tile calls this to affect it with item used on other tile
	public virtual void IndirectUseItem(Item item) {}

	protected void SendDisableTileEvent() {
		TerrainEventArgs e = new TerrainEventArgs ();
		e.tile = this;
		AIController.TriggerModifiedTerrain (e);
	}

}
