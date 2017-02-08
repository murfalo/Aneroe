using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppyRoutine : AIRoutine {

	public float followRadius = .5f;

	public override void StartRoutine(AIEntity e) {
		entity = e;
	}


	public override AIEntity.BaseState UpdateRoutine() {
		if (Vector3.Distance (entity.transform.position, PlayerController.activeCharacter.transform.position) > followRadius)
			return AIEntity.BaseState.Approaching;
		return baseState;
	}
}
