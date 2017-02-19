using System.Collections;
using SaveData;
﻿using UnityEngine;
using System;

public class Item : MonoBehaviour {

	// Components
	protected SpriteRenderer sRend;
	protected Collider2D pickupCollider;

	public string prefabName;

	protected Entity owner;

	// If true, item can be put into deep pocket
	public bool smallItem;

	// Number of units of the item
	public int count;

	/// <summary>The name that appears in this item's tooltip.</summary>
	public string Name;

	/// <summary>The description that appears in this item's tooltip.</summary>
	public string Description;
	
	public virtual void Setup () {
		sRend = GetComponent<SpriteRenderer> ();
		pickupCollider = GetComponent<Collider2D> ();
	}

	public virtual void PickupItem(Entity e) {
		owner = e;
		transform.parent = e.transform;
		transform.localPosition = new Vector3 (0, 0, 0);
		sRend.enabled = false;
		pickupCollider.enabled = false;
	}

	public virtual void DropItem(Vector3 pos) {
		owner = null;
		transform.parent = GameObject.Find ("Items").transform;
		transform.position = pos;
		sRend.enabled = true;
		pickupCollider.enabled = true;
		StartCoroutine (WaitToDespawn ());
	}

	public virtual void GiveItemTo(Transform obj) {
		owner = null;
		transform.parent = obj;
		transform.localPosition = new Vector3 (0, 0, 0);
		sRend.enabled = false;
		pickupCollider.enabled = false;
	}

	IEnumerator WaitToDespawn() {
		yield return new WaitForSeconds (30);
		if (owner == null) {
			Destroy (gameObject);
		}
	}

	public virtual void EquipItem(bool equip) {
		sRend.enabled = false;
	}

	// Returns entity that holds item
	public Entity GetEntity() {
		return owner;
	}

	public void SetEntity(Entity e) {
		owner = e;
	}

	public Sprite GetSprite() {
		return sRend.sprite;
	}

	void OnDestroy() {
		gameObject.name = "DESTROYING";
	}

	public virtual Hashtable Save() {
		Hashtable isd = new Hashtable();
		isd.Add("count", count);
		isd.Add("smallItem", smallItem);
		isd.Add("prefabName", prefabName);
		return isd;
	}

	public virtual void Load(Hashtable isd) {
		Setup ();
		count = (int)isd["count"];
		smallItem = (bool)isd["smallItem"];
		// No need to laod prefabName as this prefab already has it 
	}
}