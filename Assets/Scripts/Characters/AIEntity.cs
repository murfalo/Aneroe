using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIEntity : Entity
{
	public BaseState enemyState;

	// Idle: anything from standing still to pacing to searching actively for player
	// Approaching: anything from straight charging at the opponent to guarding a position
	// Attacking: either sending out an attack or dodging an attack (preemptive, not reactionary)
	public enum BaseState {
		Idle,
		Approaching,
		Attacking
	};

	// For each base state, a routine executes to determine how the entity acts when in that state
	Dictionary<BaseState, AIRoutine> routines;

	public override void Setup() {
		base.Setup ();
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
		return activeWeapon.GetRange ();
	}

	public void SetDir(int dir, bool isPrimary) {
		if (isPrimary) {
			primaryDir = dir;
			anim.SetInteger ("dir", primaryDir);
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
}

