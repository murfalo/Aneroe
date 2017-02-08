using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRoutine : AIRoutine {

	public float attackDelay = .5f;
	float currentDelay;
	bool resting;

	public override void StartRoutine(AIEntity e) {
		entity = e;
		resting = false;
		currentDelay = 0;
	}


	public override AIEntity.BaseState UpdateRoutine() {
		Vector3 targetVector = PlayerController.activeCharacter.transform.position - entity.transform.position;
		if (Vector3.Distance (entity.transform.position, PlayerController.activeCharacter.transform.position) > entity.GetWeaponRange ())
			return AIEntity.BaseState.Approaching;
		if (!resting) {
			// For animation
			DetermineDirections (targetVector.normalized);
			// To prevent walking
			entity.ResetDirs ();
			entity.TryAttacking ();
			resting = true;
		} else if (entity.GetState() == Entity.CharacterState.Still) {
			currentDelay += Time.deltaTime;
			if (currentDelay > attackDelay) {
				resting = false;
				currentDelay = 0;
			}
		}
		return baseState;
	}
}
