using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainEvents;

/// <summary>
/// This class controls the global states of all the enemies
/// It does NOT delegate actions to each enemy
/// </summary>
public class AIController : BaseController {

	// Functionality
	// Deals with pausing enemies when you switch between past and present
	// Spawns enemies, determining their stats, etc... and then adds Enemy controller to delegate actions to them

	public enum Difficulty
	{
		Easy = 0,
		Normal,
		Hard,
		Nope,
		Really_Though
	};

	public static Difficulty difficulty = Difficulty.Normal;
	public static float[] difficultyModifiers = new float[5] {.75f, 1, 1.25f, 2, 3};
	//public static StatInfo[] difficultyBoosts = new StatInfo[System.Enum.GetNames(typeof(Difficulty)).Length];

	public static SpawnerController[] allSpawners;
	public static List<SpawnerController> spawners;

	public static event EventHandler<TerrainEventArgs> modifiedTerrain;

	public override void InternalSetup() {
		spawners = new List<SpawnerController> ();
		modifiedTerrain += HandleNewTerrain;
	}

	public override void ExternalSetup() {
		SceneController.timeSwapped += PauseEnemies;
		SceneController.mergedNewScene += GrabAllSpawners;
	}

	public override void RemoveEventListeners() {
		SceneController.timeSwapped -= PauseEnemies;
		SceneController.mergedNewScene -= GrabAllSpawners;
		modifiedTerrain -= HandleNewTerrain;
	}

	public static float GetDifficultyModifier() {
		return difficultyModifiers [(int)difficulty];
	}

	public static void AddSpawner(SpawnerController spawner) {
		if (!spawners.Contains (spawner)) {
			spawners.Add (spawner);
		}
	}

	public static void RemoveSpawner(SpawnerController spawner) {
		spawners.Remove (spawner);
	}

	public void PauseEnemies(object sender, PlayerSwitchEventArgs e) {
		foreach (SpawnerController spawner in spawners) {
		}
	}

	public static void TriggerModifiedTerrain(TerrainEventArgs e) {
		if (AIController.modifiedTerrain != null)
			AIController.modifiedTerrain (e.tile, e);
	}

	public void GrabAllSpawners(object sender, EventArgs e) {
		allSpawners = GameObject.Find("Spawners").GetComponentsInChildren<SpawnerController> ();
	}

	public void HandleNewTerrain(object sender, TerrainEventArgs e) {
		Vector3 tilePos = e.tile.transform.position;
		SpawnerController closestSpawner = null;
		float closest = Mathf.Infinity;
		float dist;
		foreach (SpawnerController spawner in allSpawners) {
			dist = Vector3.Distance (spawner.transform.position, tilePos);
			if (dist < closest) {
				closest = dist;
				closestSpawner = spawner;
			}
		}
		if (closestSpawner != null) {
			foreach (Waypoint wp in closestSpawner.GetWaypoints()) {
				wp.RecalculateNeighbors ();
			}
		}
	}
}

