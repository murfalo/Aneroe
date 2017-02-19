using UnityEngine;
using System.Linq;
using System.Collections;
using TerrainEvents;

public class Tile : MonoBehaviour {

	public Tile otherTile;

	[HideInInspector]
	public bool primaryTile = true;

	[HideInInspector]
	public PlayerEntity.CharacterState interactState;

	public System.Type[] usableItemTypes;

	public virtual bool CanUseItem(Item item) {
		// If you're not wielding something, interaction is always allowed
		if (item == null)
			return true;
		System.Type itemType = item.GetType ();
		return usableItemTypes.Any(i => i.Equals(itemType));
	}

	public PlayerEntity.CharacterState GetInteractState() {
		return interactState;
	}

	// Use item on tile
	// Returns false if the item still belongs to the entity
	public virtual void UseItem(Item itemUsed, out Item newItem) {
		newItem = null;
	}

	protected void SendDisableTileEvent() {
		TerrainEventArgs e = new TerrainEventArgs ();
		e.tile = this;
		AIController.TriggerModifiedTerrain (e);
	}

	public virtual Hashtable Save() { return new Hashtable (); }

	public virtual void Load(Hashtable tsd) {}

}
