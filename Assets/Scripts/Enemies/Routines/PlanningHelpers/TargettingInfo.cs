using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargettingInfo {

	public AIEntity entity;
	public int assignedDirection;

	public TargettingInfo (AIEntity ai, int dir) {
		entity = ai;
		assignedDirection = dir;
	}
}
