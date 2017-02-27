using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRoutine : AIRoutine {

	float currentDelay;
	bool resting;

	const float rangeFactor = .9f;

	public override void StartRoutine(AIEntity e) {
		entity = e;
		resting = false;
		currentDelay = Random.Range(-.5f,0f);
	}


	public override AIEntity.BaseState UpdateRoutine() {
		Vector3 targetVector = PlayerController.activeCharacter.transform.position - entity.transform.position;
		if (Vector3.Distance (entity.transform.position, PlayerController.activeCharacter.transform.position) > rangeFactor * entity.GetWeaponRange ())
			return AIEntity.BaseState.Approaching;
		if (!resting) {
			if (currentDelay < 0) {
				currentDelay += Time.deltaTime;
			} else if (entity.CanActOutOfMovement()) {
				// For animation
				DetermineDirections (targetVector.normalized);
				// To prevent walking
				entity.ResetDirs ();
				entity.TryAttacking ();
				resting = true;
				currentDelay = Random.Range(-.5f,0f);
			}
		} else {
			currentDelay += Time.deltaTime;
			if (currentDelay > 0) {
				resting = false;
			}
		}
		return baseState;
	}
}
