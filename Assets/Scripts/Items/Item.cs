using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	// Components

	SpriteRenderer sRend;
	Collider2D pickupCollider;
	public RuntimeAnimatorController weaponAnim;

	// Properties

	// If true, item can be put into deep pocket
	public bool smallItem;

	// Weight of item. Prevents too many items from being held in a backpack
	public float weight;

	void Start () {
	}

	void Update () {
	
	}

	// If a character picks up an item, it is moved in the heirarchy to a child of their gameobject
	public virtual void PickupItem(Character c) {
		// Add code to add the item to the backpack
		transform.parent = c.transform;
	}
}
