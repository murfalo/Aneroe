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

	public static Dictionary<Entity, List<TargettingInfo>> targetting;
	public static Entity updateTargettingsOf;

	public override void InternalSetup() {
		spawners = new List<SpawnerController> ();
		modifiedTerrain += HandleNewTerrain;
	}

	public override void ExternalSetup() {
		SceneController.timeSwapped += PauseEnemies;
		SceneController.mergedNewScene += GrabAllSpawners;
	}

	void Update() {
		if (updateTargettingsOf != null) {
			UpdateTargettings (updateTargettingsOf);
			updateTargettingsOf = null;
		}
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

	public static void PauseAllEnemies(bool pause) {
		GameObject.Find ("Control").GetComponent<AIController> ().PauseEnemies (null, new PlayerSwitchEventArgs (null, pause ? null : PlayerController.activeCharacter));
	}

	public void PauseEnemies(object sender, PlayerSwitchEventArgs e) {
		foreach (SpawnerController spawner in spawners) {
			spawner.HandleSpawnerActivity (e);
		}
	}

	public static void TriggerModifiedTerrain(TerrainEventArgs e) {
		if (AIController.modifiedTerrain != null)
			AIController.modifiedTerrain (e.tile, e);
	}

	public void GrabAllSpawners(object sender, EventArgs e) {
		allSpawners = null;
		//allSpawners = GameObject.Find("Spawners").GetComponentsInChildren<SpawnerController> ();
	}

	public void HandleNewTerrain(object sender, TerrainEventArgs e) {
		if (allSpawners == null)
			return;
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

	public static void AddToTargetting(Entity target, TargettingInfo tI) {
		if (!targetting.ContainsKey (target))
			targetting.Add (target, new List<TargettingInfo> ());
		targetting[target].Add (tI);

		// Have AI Controller process new targetter next update cycle
		updateTargettingsOf = target;
	}

	public static void RemoveFromTargetting(Entity target, TargettingInfo tI) {
		if (!targetting.ContainsKey (target))
			targetting.Add (target, new List<TargettingInfo> ());
		targetting[target].Remove (tI);
	}

	// To be used in spreading out enemies around target
	void UpdateTargettings(Entity target) {
		// For each direction and AIEntity, the dist to target from that direction
		/*
		float[,] dirDist = new float[4,targetting[target].Count];
		for (int tI = 0; tI < targetting[target].Count; tI++) {
			AIEntity aiE = targetting [target] [tI].entity;
			Vector2 dirVec = aiE.DirFacingTo (target);
			for (int i = 0; i < 4; i++) {
				// for each direction
				dirDist[i,tI] = Vector2.Distance((Vector2)aiE.transform.position,target + dirVec * aiE.GetWeaponRange());
			}
		}
		*/
	}
}

