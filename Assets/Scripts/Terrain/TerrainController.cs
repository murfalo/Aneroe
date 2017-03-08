using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TerrainController : MonoBehaviour {

	Transform sceneHolder;

	[HideInInspector]
	public GameObject manMadeItemHolder;

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

		Hashtable manMadeTiles = new Hashtable ();
		if (manMadeItemHolder != null) {
			tileComponents = manMadeItemHolder.GetComponentsInChildren<Tile> ();
			for (int i = 0; i < tileComponents.Length; i++) {
				manMadeTiles.Add(tileComponents[i].name,tileComponents [i].Save ());
			}
		}
		GameController.SetSaveValue (sceneHolder.name + "ManMadeTerrain", manMadeTiles);
	}

	public void Load(object sender, SceneSwitchEventArgs e) {
		sceneHolder = transform.root;
		Hashtable tiles;
		GameController.GetSaveValue (sceneHolder.name + "Terrain", out tiles);
		if (tiles != null) {
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

		// Get rid of old tiles, even if they weren't changed since the previous load
		if (manMadeItemHolder != null)
			Destroy (manMadeItemHolder);
		manMadeItemHolder = new GameObject ();
		manMadeItemHolder.transform.SetParent (sceneHolder);
		GameController.GetSaveValue (sceneHolder.name + "ManMadeTerrain", out tiles);
		if (tiles != null) {
			foreach (KeyValuePair<string, object> pair in tiles) {
				Hashtable tileSave = (Hashtable)pair.Value;
				Tile tile = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Terrain/" + tileSave ["prefabName"])).GetComponent<Tile>();
				tile.transform.SetParent (manMadeItemHolder.transform);
				tile.name = pair.Key;
				tile.Load (tileSave);
			}
		}
	}
}
