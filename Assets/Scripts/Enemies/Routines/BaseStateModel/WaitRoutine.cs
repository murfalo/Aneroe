using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitRoutine : AIRoutine {

	public float radiusOfSight;

	public override void StartRoutine(AIEntity e) {
		entity = e;
	}

	public override AIEntity.BaseState UpdateRoutine() {
		if (radiusOfSight >= Vector3.Distance (entity.transform.position, PlayerController.activeCharacter.transform.position)) {
			return AIEntity.BaseState.Approaching;
		}
		return baseState;
	}
}
