using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIEntity : Entity
{
	// Idle: anything from standing still to pacing to searching actively for player
	// Approaching: anything from straight charging at the opponent to guarding a position
	// Attacking: either sending out an attack or dodging an attack (preemptive, not reactionary)
	public enum BaseState {
		Idle,
		Approaching,
		Attacking
	};

	public BaseState enemyState;

	public string[] itemDropNames;
	public float[] itemDropWeights;

	// For each base state, a routine executes to determine how the entity acts when in that state
	Dictionary<BaseState, AIRoutine> routines;

	public override void Setup() {
		base.Setup ();
		foreach (string itemName in defaultItemPrefabNames) {
			var itemObj = GameObject.Instantiate (Resources.Load<GameObject> ("Prefabs/Items/" + itemName));
			itemObj.transform.SetParent (transform, false);
			Item i = itemObj.GetComponentInChildren<Item> ();
			i.Setup();	
			i.PickupItem (this);
		}
		activeItem = GetComponentInChildren<Weapon> ();
		activeItem.EquipItem (true);

		controller = GetComponentInParent<EntityController> ();
		routines = new Dictionary<BaseState, AIRoutine> ();

		foreach (AIRoutine routine in GetComponentsInChildren<AIRoutine>()) {
			routines.Add (routine.baseState, routine);
		}

		enemyState = BaseState.Idle;
		routines [enemyState].StartRoutine (this);
	}

	public void UpdateEntity() {
		//print (gameObject.name + "  " + stats.GetStat ("health") + "  " + (CharacterState)anim.GetInteger ("state"));
		BaseState oldState = enemyState;
		enemyState = routines [enemyState].UpdateRoutine ();
		if (enemyState != oldState) {
			routines [enemyState].StartRoutine (this);
		}
	}

	public override void DoFixedUpdate() {
		base.DoFixedUpdate ();
	}

	public float GetWeaponRange() {
		return ((Weapon)activeItem).GetRange ();
	}

	public void SetDirs(int dir, bool isPrimary) {
		if (isPrimary) {
			primaryDir = dir;
			SetDir(primaryDir);
		} else {
			secondaryDir = dir;	
		}
	}

	public void ResetDirs() {
		primaryDir = 0;
		secondaryDir = 0;
	}

	public SpawnerController GetController() {
		return (SpawnerController)controller;
	}

	public void NotifyController(string action) {
		controller.RespondToEntityAction (this, action);
	}

	public GameObject[] GetDrops() {
		GameObject[] items = new GameObject[itemDropNames.Length];
		for (int i = 0; i < itemDropNames.Length; i++) {
			items [i] = (GameObject)Resources.Load ("Prefabs/Items/" + itemDropNames [i]);
		}
		return items;
	}

	public int GetRandomItemIndex() {
		float rand = Random.Range (0, 1f);
		for (int i = 0; i < itemDropWeights.Length; i++) {
			float weight = itemDropWeights [i];
			if (rand < weight)
				return i;
			rand -= weight;
		}
		return 0;
	}

	public void ToggleEnabled(bool enabled) {
		foreach (Animator anim in GetComponentsInChildren<Animator>())
			anim.enabled = enabled;
	}
}

