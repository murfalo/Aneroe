using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Components
	protected SpriteRenderer sRend;
	protected Collider2D pickupCollider;

	// Properties
	protected Entity owner;

	// If true, item can be put into deep pocket
	public bool smallItem;

	// Weight of item. Prevents too many items from being held in a backpack
	public float weight;

	void Start () {
		sRend = GetComponent<SpriteRenderer> ();
	}

	void Update () {
	
	}

	// If a Entity picks up an item, it is moved in the heirarchy to a child of their gameobject
	public virtual void PickupItem(Entity e) {
		// Add code to add the item to the backpack
		transform.parent = e.transform;
		owner = e;
	}

	// Returns entity that holds item
	public Entity GetEntity() {
		return owner;
	}
}
