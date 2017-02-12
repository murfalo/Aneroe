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

	// Number of units of the item
	public int count;

	protected virtual void Awake () {
		sRend = GetComponent<SpriteRenderer> ();
		pickupCollider = GetComponent<Collider2D> ();
	}

	public virtual void PickupItem(Entity e) {
		owner = e;
		sRend.enabled = false;
		pickupCollider.enabled = false;
	}

	public virtual void DropItem(Vector3 pos) {
		owner = null;
		transform.position = pos;
		sRend.enabled = true;
		pickupCollider.enabled = true;
	}

	// Returns entity that holds item
	public Entity GetEntity() {
		return owner;
	}

	public Sprite GetSprite() {
		return sRend.sprite;
	}
}
