using UnityEngine;
using System.Collections;

public class ItemMound : Tile {

	Item buriedItem;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Bury item in mound
	public override void UseItem(Item item) {}

	// Other tile calls this to affect it with item buried in other tile
	public override void IndirectUseItem(Item item) {}

	// Dig up item in mound
	public Item RetrieveItem(Entity c) {
		Item i = buriedItem;
		buriedItem = null;
		return i;
	}
}
