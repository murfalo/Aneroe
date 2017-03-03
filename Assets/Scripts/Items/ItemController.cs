using UnityEngine;
using System;
using System.Collections;

public class ItemController : MonoBehaviour
{
	Transform sceneHolder;

	public void Awake() {
		GameController.fileLoaded += Load;
		GameController.fileSaving += Save;
	}

	public void OnDestroy() {
		GameController.fileLoaded -= Load;
		GameController.fileSaving -= Save;
	}

	public void Save(object sender, EventArgs e) {
		sceneHolder = transform.root;
		Hashtable items = new Hashtable ();
		Item[] itemComponents = GetComponentsInChildren<Item> ();
		for (int i = 0; i < itemComponents.Length; i++) {
			Item item = itemComponents [i];
			Hashtable tsd = item.Save ();
			items.Add (item.name, tsd);
		}
		GameController.SetSaveValue (sceneHolder.name + "LocalItems", items);
	}

	public void Load(object sender, SceneSwitchEventArgs e) {
		sceneHolder = transform.root;
		Hashtable items;
		GameController.GetSaveValue (sceneHolder.name + "LocalItems", out items);
		if (items == null)
			return;
		Item[] itemComponents = sceneHolder.GetComponentsInChildren<Item> ();
		for (int i = 0; i < itemComponents.Length; i++) {
			Item item = itemComponents [i];
			// Destroy item if it wasn't saved, because that means it shouldn't exist
			if (items.ContainsKey (item.name))
				item.Load ((Hashtable)items [item.name]);
			else
				Destroy (item.gameObject);
		}
	}
}

