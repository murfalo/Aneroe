using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekOutRoutine : AIRoutine {

	Waypoint[] waypointNetwork;
	List<Waypoint> path;
	Vector3 pathedEndPos;
	float currentPathAcc;
	// How much further opponent needs to get from current path calculated accuracy to trigger new path calculation
	static float pathTol = 2f;

	public override void StartRoutine(AIEntity e) {
		entity = e;
		waypointNetwork = entity.GetController ().GetWaypoints ();
		pathedEndPos = new Vector3 (0, 0, 0);
		path = new List<Waypoint>();
	}

	public override AIEntity.BaseState UpdateRoutine() {
		Vector3 playerPos = PlayerController.activeCharacter.transform.position;
		Vector3 targetVector =  playerPos - entity.transform.position;
		if (targetVector.magnitude < entity.GetWeaponRange ())
			return AIEntity.BaseState.Attacking;

		RaycastHit2D obstacle = Physics2D.Raycast (entity.transform.position, targetVector.normalized, targetVector.magnitude, 1 << LayerMask.NameToLayer ("Wall"));
		//Debug.DrawLine (entity.transform.position, PlayerController.activeCharacter.transform.position, Color.black, .1f);
		if (obstacle.collider != null) {
			// If opponent has strayed too far from previous calculated path, figure out a new path to opponent
			if ((playerPos - pathedEndPos).magnitude > currentPathAcc + pathTol)
				path = new List<Waypoint>();
			targetVector = ContinueOnPath (playerPos, targetVector);
		}

		DetermineDirections (targetVector.normalized);
		entity.TryWalk ();
		return baseState;
	}

	Vector3 ContinueOnPath(Vector3 playerPos, Vector3 defaultVec) {
		if (path.Count == 0) {
			path = Waypoint.FindPath (waypointNetwork, entity.transform.position, playerPos);
			pathedEndPos = path [path.Count - 1].transform.position;
			currentPathAcc = (playerPos - pathedEndPos).magnitude;
			if (path.Count == 0)
				return defaultVec;
		}
		Vector3 target = path [0].transform.position - entity.transform.position;
		if (target.magnitude < entity.GetEntityStat ("speed") * Time.fixedDeltaTime) {
			path.RemoveAt (0);
			if (path.Count > 0) {
				target = path [0].transform.position - entity.transform.position;
			} else {
				return defaultVec;
			}
		}
		return target;
	}
}
