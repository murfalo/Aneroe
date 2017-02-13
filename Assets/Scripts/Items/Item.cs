using UnityEngine;
using System.Collections;
using SaveData;

public class Item : MonoBehaviour, ISavable<ItemSaveData> {

	// Components
	protected SpriteRenderer sRend;
	protected Collider2D pickupCollider;

	// Properties
	public string prefabName;

	protected Entity owner;

	// If true, item can be put into deep pocket
	public bool smallItem;

	// Number of units of the item
	public int count;

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

	IEnumerator WaitToDespawn() {
		yield return new WaitForSeconds (30);
		if (owner == null) {
			Destroy (gameObject);
		}
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

	public virtual ItemSaveData Save(ItemSaveData baseObj) {
		ItemSaveData isd;
		if (baseObj != default(ItemSaveData))
			isd = baseObj;
		else 
			isd = new ItemSaveData ();
		isd.count = count;
		isd.smallItem = smallItem;
		isd.prefabName = prefabName;
		return isd;
	}

	public virtual void Load(ItemSaveData isd) {
		Setup ();
		count = isd.count;
		smallItem = isd.smallItem;
		// No need to laod prefabName as this prefab already has it 
	}
}