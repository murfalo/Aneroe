using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : EntityController {

	public GameObject[] enemyTypes;

	SpawnPoint[] spawnPoints;
	List<AIEntity> spawnedEntities;
	Waypoint[] waypointNetwork;

	Entity targetToKill;
	bool paused;

	void Awake () {
		spawnPoints = GetComponentsInChildren<SpawnPoint> ();	
		spawnedEntities = new List<AIEntity> ();		
		actionResponses = new Dictionary<string, System.Action<Entity>> () {
			{"die",DestroyEntity}
		};
		//waypointNetwork = GetComponentsInChildren<Waypoint> ();
		waypointNetwork = GameObject.Find("Waypoints").GetComponentsInChildren<Waypoint>();
		foreach (Waypoint wp in waypointNetwork) {
			wp.Setup (waypointNetwork);
		}

		paused = false;
	}
		
	void Update() {
		if (paused)
			return;
		foreach (AIEntity e in spawnedEntities) {
			e.UpdateEntity ();
		}
	}

	void FixedUpdate() {
		if (paused)
			return;
		foreach (AIEntity e in spawnedEntities) {
			e.DoFixedUpdate ();
		}
	}

	protected virtual void Spawn() {
		foreach (SpawnPoint sp in spawnPoints) {
			for (int i = 0; i < sp.enemyIDs.Length; i++) {
				GameObject enemy = GameObject.Instantiate(enemyTypes [sp.enemyIDs [i]],transform);
				enemy.transform.position = sp.transform.position + (Vector3)sp.spawnOffsets [i];
				AIEntity entity = enemy.GetComponent<AIEntity> ();
				entity.Setup ();
				entity.stats.ModifyStatsByFactor (AIController.GetDifficultyModifier ());
				spawnedEntities.Add (entity);
				AIController.AddSpawner (this);
			}
		}
		// Deactivate spawner collider to prevent respawning
		// Realize future SpawnerControllers can inherit from SpawnerController and override Spawn()
		GetComponent<Collider2D> ().enabled = false;
	}

	public void HandleSpawnerActivity(PlayerSwitchEventArgs e) {
		paused = e.newPlayer != targetToKill;
		foreach (AIEntity aiE in spawnedEntities) {
			aiE.ToggleEnabled (!paused);
		}
	}

	void DestroyEntity(Entity e) {
		GameObject[] itemsToSpawn = ((AIEntity)e).GetDrops();
		GameObject item = (GameObject)Instantiate(itemsToSpawn [((AIEntity)e).GetRandomItemIndex ()]);
		Item i = item.GetComponent<Item> ();
		i.Setup ();
		i.DropItem (e.transform.position);
		spawnedEntities.Remove ((AIEntity)e);
		if (spawnedEntities.Count == 0) {
			AIController.RemoveSpawner (this);
		}
		Destroy (e.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals ("Character")) {
			targetToKill = other.GetComponent<Entity> ();
			Spawn();
		}
	}

	public Waypoint[] GetWaypoints() {
		return waypointNetwork;
	}
}
