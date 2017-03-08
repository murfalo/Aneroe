using UnityEngine;
using System.Linq;
using System.Collections;
using TerrainEvents;

public class Tile : MonoBehaviour {

	public string prefabName;

	public Tile otherTile;

	[HideInInspector]
	public bool primaryTile = true;

	[HideInInspector]
	public Entity.CharacterState interactState;

	public System.Type[] usableItemTypes;

	public virtual bool CanUseItem(Item item) {
	    return (item == null) ? true : usableItemTypes.Any(i => i == item.GetType ());
	}

	public Entity.CharacterState GetInteractState() {
		return interactState;
	}

	// Use item on tile
	// Returns false if the item still belongs to the entity
	public virtual bool UseItem(Item itemUsed, out Item newItem) {
		newItem = null;
		return true;
	}

    public virtual void IndirectUseItem(Item item, out Item newItem)
    {
        newItem = typeof(Weapon) == item.GetType() ? item : null;
    }

    protected void SendDisableTileEvent() {
	    var e = new TerrainEventArgs {tile = this};
	    AIController.TriggerModifiedTerrain (e);
	}

	public virtual Hashtable Save() { return new Hashtable (); }

	public virtual void Load(Hashtable tsd) {}

}