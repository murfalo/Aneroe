using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	Entity target;

	void Update() {
		if (target != null) {
			transform.position = target.transform.position + new Vector3(0,0,-10);
		}
	}

	public void SetTarget(Entity t) {
		target = t;
	}
}
