using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekOutRoutine : AIRoutine {

	public float radiusOfSight;
	Waypoint[] waypointNetwork;
	List<Waypoint> path;

	public override void StartRoutine(AIEntity e) {
		entity = e;
		waypointNetwork = entity.GetController ().GetWaypoints ();
		path = Waypoint.FindPath (waypointNetwork, entity.transform.position, PlayerController.activeCharacter.transform.position);
	}

	public override AIEntity.BaseState UpdateRoutine() {
		if (path.Count > 0) {
		//	entity.SetDirections(DetermineDirections())
		}
		return baseState;
	}
}
