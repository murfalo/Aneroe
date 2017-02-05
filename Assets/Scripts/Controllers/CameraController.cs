using UnityEngine;
using System.Collections;

public class CameraController : BaseController {

	Entity target;
	GameObject cam;

	public override void InternalSetup() {
		cam = GameObject.Find ("Main Camera");
	}

	void Update() {
		if (target != null) {
			cam.transform.position = target.transform.position + new Vector3(0,0,-10);
		}
	}

	public void SetTarget(Entity t) {
		target = t;
	}
}
