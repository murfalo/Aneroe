using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainController : BaseController {

	Transform sceneHolder;

	public void Awake() {
		sceneHolder = gameObject.transform.root;
		SaveController.fileLoaded += Load;
		SaveController.fileSaving += Save;
	}

	public void OnDestroy() {
		SaveController.fileLoaded -= Load;
		SaveController.fileSaving -= Save;
	}

	public void Save(object sender, EventArgs e) {
		Hashtable tiles = new Hashtable ();
		Tile[] tileComponents = sceneHolder.GetComponentsInChildren<Tile> ();
		for (int i = 0; i < tileComponents.Length; i++) {
			Tile t = tileComponents [i];
			Hashtable tsd = t.Save ();
			tiles.Add (t.name + t.transform.parent.name, tsd);
		}
		SaveController.SetValue (sceneHolder.name + "Terrain", tiles);
	}

	public void Load(object sender, SceneSwitchEventArgs e) {
		Hashtable tiles;
		SaveController.GetValue (sceneHolder.name + "Terrain", out tiles);
		if (tiles == null)
			return;
		Tile[] tileComponents = sceneHolder.GetComponentsInChildren<Tile> ();
		for (int i = 0; i < tileComponents.Length; i++) {
			Tile t = tileComponents [i];
			if (t.primaryTile) {
				string saveName = t.name + t.transform.parent.name;
				if (tiles.ContainsKey (saveName))
					t.Load ((Hashtable)tiles [saveName]);
				saveName = t.otherTile.name + t.otherTile.transform.parent.name;
				if (tiles.ContainsKey(saveName))
					t.otherTile.Load ((Hashtable)tiles [saveName]);
			}
		}
	}
}
