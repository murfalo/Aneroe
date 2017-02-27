using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIRoutine : MonoBehaviour {

	public AIEntity.BaseState baseState;

	protected AIEntity entity;

	public virtual void StartRoutine(AIEntity e) {
		// use to initialize entity and other fields
	}

	public virtual AIEntity.BaseState UpdateRoutine() {
		// Never switch away from this routine and do nothing in it.
		// Pretty useless, eh? That's right. 
		// Don't use this. Extend it.
		return baseState;
	}

	protected void DetermineDirections(Vector3 path) {
		float maxDot = 0;
		int maxIndex = -1;
		float diff = 1;
		for (int i = 0; i < 4; i++) {
			diff = Vector3.Dot (path, (Vector3)Entity.directionVectors [i]);
			if (diff > maxDot) {
				maxDot = diff;
				maxIndex = i;
			}
		}
		entity.SetDirs (maxIndex + 1, true);
		int lowerIndex = ((maxIndex - 1 % 4) + 4) % 4;
		diff = Vector3.Dot (path, (Vector3)Entity.directionVectors [maxIndex] + Entity.secondaryDirFactor * (Vector3)Entity.directionVectors [lowerIndex]);
		if (maxDot < diff) {
			entity.SetDirs (lowerIndex + 1, false);
			maxDot = diff;
		} 
		int higherIndex = (maxIndex + 1) % 4;
		diff = Vector3.Dot (path, (Vector3)Entity.directionVectors [maxIndex] + Entity.secondaryDirFactor * (Vector3)Entity.directionVectors [higherIndex]);
		if (maxDot < diff) {
			entity.SetDirs (higherIndex + 1, false);
		}
	}

}
