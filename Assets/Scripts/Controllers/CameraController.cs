using UnityEngine;
using System.Collections;

public class CameraController : BaseController {

	Entity target;
	GameObject cam;

	public override void InternalSetup() {
		cam = GameObject.Find ("Main Camera");
	}

	public override void ExternalSetup() {
		SceneController.timeSwapped += SetTarget;
	}

	void Update() {
		if (target != null) {
			cam.transform.position = target.transform.position + new Vector3(0,0,-10);
		}
	}

	void SetTarget(object sender, System.EventArgs e) {
		target = PlayerController.activeCharacter;
	}
}
