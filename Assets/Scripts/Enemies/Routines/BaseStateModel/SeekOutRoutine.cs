using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekOutRoutine : AIRoutine {

	public enum SeekState
	{
		OnHold,
		ToTarget,
		ToPos
	};

	// Handling entity path collisions
	public SeekState seekState;
	public List<AIEntity> waitingOn;
	List<Vector3> customManeuver;

	// Handling cross-entity strategizing
	TargettingInfo targettingInfo;

	Waypoint[] waypointNetwork;
	List<Waypoint> path;
	Vector3 pathedEndPos;
	float currentPathAcc;

	// How much further opponent needs to get from current path calculated accuracy to trigger new path calculation
	const float pathTol = 2f;
	const float rangeFactor = 1.5f;

	public override void StartRoutine(AIEntity e) {
		entity = e;
		waypointNetwork = entity.GetController ().GetWaypoints ();
		pathedEndPos = new Vector3 (0, 0, 0);
		path = new List<Waypoint>();

		seekState = SeekState.ToTarget;
		waitingOn = new List<AIEntity>();
	}

	public override AIEntity.BaseState UpdateRoutine() {
		Vector3 playerPos = PlayerController.activeCharacter.transform.position;
		Vector3 targetVector =  playerPos - entity.transform.position;
		if (targetVector.magnitude < rangeFactor * entity.GetWeaponRange ())
			return AIEntity.BaseState.Attacking;

		switch (seekState) {
		case SeekState.ToPos:
			Vector3 nextVec = customManeuver [0];
			if (Vector3.Distance (entity.transform.position, nextVec) < StepSize ()) {
				customManeuver.RemoveAt (0);
				if (customManeuver.Count == 0) {
					seekState = SeekState.ToTarget;
					goto case SeekState.ToTarget;
				} else
					nextVec = customManeuver [0];
			}
			DetermineDirections (nextVec.normalized);
			break;
		case SeekState.ToTarget:
			RaycastHit2D obstacle = Physics2D.Raycast (entity.transform.position, targetVector.normalized, targetVector.magnitude, 1 << LayerMask.NameToLayer ("Wall"));
			//Debug.DrawLine (entity.transform.position, PlayerController.activeCharacter.transform.position, Color.black, .1f);
			if (obstacle.collider != null) {
				// If opponent has strayed too far from previous calculated path, figure out a new path to opponent
				if ((playerPos - pathedEndPos).magnitude > currentPathAcc + pathTol)
					path = new List<Waypoint> ();
				targetVector = ContinueOnPath (playerPos, targetVector);
			}

			DetermineDirections (targetVector.normalized);
			break;
		}

		entity.TryWalk ();
		return baseState;
	}

	Vector3 ContinueOnPath(Vector3 playerPos, Vector3 defaultVec) {
		if (path.Count == 0) {
			path = Waypoint.FindPath (waypointNetwork, entity.transform.position, playerPos);
			if (path.Count == 0)
				return defaultVec;
			pathedEndPos = path [path.Count - 1].transform.position;
			currentPathAcc = (playerPos - pathedEndPos).magnitude;
		}
		Vector3 target = path [0].transform.position - entity.transform.position;
		if (target.magnitude < StepSize()) {
			path.RemoveAt (0);
			if (path.Count > 0) {
				target = path [0].transform.position - entity.transform.position;
			} else {
				return defaultVec;
			}
		}
		return target;
	}

	float StepSize() {
		return entity.GetEntityStat ("speed") * Time.fixedDeltaTime;
	}

	void OnTriggerEnter2D(Collider2D other) {
		AIEntity otherE = other.GetComponentInParent<AIEntity> ();
		if (otherE == null)
			return;
		if (otherE.enemyState != AIEntity.BaseState.Approaching) {
			// Find path around
		} else {
			// Figure out who goes first based on directions

		}
	}

	void OnTriggerExit2D(Collider2D other) {
		AIEntity otherE = other.GetComponentInParent<AIEntity> ();
		if (otherE == null)
			return;
		if (otherE.enemyState == AIEntity.BaseState.Approaching) {
			if (waitingOn.Contains(otherE)) {
				waitingOn.Remove (otherE);
				CheckToStopWaiting ();
			}
		}
	}

	void CheckToStopWaiting() {

	}
}
