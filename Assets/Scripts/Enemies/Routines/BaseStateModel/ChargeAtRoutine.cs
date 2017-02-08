using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAtRoutine : AIRoutine {

	public override void StartRoutine(AIEntity e) {
		entity = e;
	}

	public override AIEntity.BaseState UpdateRoutine() {
		Vector3 targetVector = PlayerController.activeCharacter.transform.position - entity.transform.position;
		if (Vector3.Distance (entity.transform.position, PlayerController.activeCharacter.transform.position) < entity.GetWeaponRange ())
			return AIEntity.BaseState.Attacking;

		DetermineDirections (targetVector.normalized);
		entity.TryWalk ();
		return baseState;
	}
}
