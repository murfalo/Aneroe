using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : EntityController {

	public GameObject[] enemyTypes;

	SpawnPoint[] spawnPoints;
	List<AIEntity> spawnedEntities;
	Waypoint[] waypointNetwork;

	void Awake () {
		spawnPoints = GetComponentsInChildren<SpawnPoint> ();	
		spawnedEntities = new List<AIEntity> ();		
		actionResponses = new Dictionary<string, System.Action<Entity>> () {
			{"die",DestroyEntity}
		};
		waypointNetwork = GetComponentsInChildren<Waypoint> ();
		foreach (Waypoint wp in waypointNetwork) {
			wp.Setup (waypointNetwork);
		}
	}
		
	void Update() {
		foreach (AIEntity e in spawnedEntities) {
			e.UpdateEntity ();
		}
	}

	void FixedUpdate() {
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

	void DestroyEntity(Entity e) {
		spawnedEntities.Remove ((AIEntity)e);
		if (spawnedEntities.Count == 0) {
			AIController.RemoveSpawner (this);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag.Equals ("Character")) {
			Spawn();
		}
	}

	public Waypoint[] GetWaypoints() {
		return waypointNetwork;
	}
}
