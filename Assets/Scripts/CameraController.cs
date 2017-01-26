using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Character target;

	void Update() {
		if (target != null) {
			transform.position = target.transform.position + new Vector3(0,0,-10);
		}
	}

	public void SetTarget(Character t) {
		target = t;
	}
}
