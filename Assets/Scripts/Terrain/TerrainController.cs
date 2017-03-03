using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainController : MonoBehaviour {

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
		Hashtable tiles = new Hashtable ();
		Tile[] tileComponents = sceneHolder.GetComponentsInChildren<Tile> ();
		for (int i = 0; i < tileComponents.Length; i++) {
			Tile t = tileComponents [i];
			Hashtable tsd = t.Save ();
			tiles.Add (t.name + t.transform.parent.name, tsd);
		}
		GameController.SetSaveValue (sceneHolder.name + "Terrain", tiles);
	}

	public void Load(object sender, SceneSwitchEventArgs e) {
		sceneHolder = transform.root;
		Hashtable tiles;
		GameController.GetSaveValue (sceneHolder.name + "Terrain", out tiles);
		if (tiles == null)
			return;
		Tile[] tileComponents = sceneHolder.GetComponentsInChildren<Tile> ();
		for (int i = 0; i < tileComponents.Length; i++) {
			Tile t = tileComponents [i];
			if (t.primaryTile) {
				string saveName = t.name + t.transform.parent.name;
				if (tiles.ContainsKey (saveName))
					t.Load ((Hashtable)tiles [saveName]);
				if (t.otherTile != null) {
					saveName = t.otherTile.name + t.otherTile.transform.parent.name;
					if (tiles.ContainsKey (saveName))
						t.otherTile.Load ((Hashtable)tiles [saveName]);
				}
			}
		}
	}
}
