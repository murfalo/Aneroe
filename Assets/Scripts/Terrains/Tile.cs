using UnityEngine;
using System.Linq;
using System.Collections;
using TerrainEvents;

public class Tile : MonoBehaviour {

	public Tile otherTile;

	[HideInInspector]
	public bool primaryTile = true;

	[HideInInspector]
	public Entity.CharacterState interactState;

	public System.Type[] usableItemTypes;

    public string[] usableItemPrefabNames;

	public virtual bool CanUseItem(Item item) {
		// If you're not wielding something, interaction is always allowed
		if (item == null)
			return true;
		var itemType = item.GetType ();
	    return itemType == System.Type.GetType("Item") ? usableItemPrefabNames.Any(n => n == item.prefabName) : usableItemTypes.Any(i => i == itemType);
	}

	public Entity.CharacterState GetInteractState() {
		return interactState;
	}

	// Use item on tile
	// Returns false if the item still belongs to the entity
	public virtual void UseItem(Item itemUsed, out Item newItem) {
		newItem = null;
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
