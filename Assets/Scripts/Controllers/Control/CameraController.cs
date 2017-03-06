using UnityEngine;
using System.Collections;
using AneroeInputs;

public class CameraController : BaseController {

	// For non-cutscene entity trailing
	Entity target;

	// For cutscene camera movement
	public float camSpeed = 4f;

	static bool cutSceneMovement;
	static Vector3 targetPos;
	static Vector3 vecToTargetPos;
	static GameObject cam;

	public override void InternalSetup() {
		cam = GameObject.Find ("Main Camera");
		cutSceneMovement = false;
	}

	public override void ExternalSetup() {
		SceneController.timeSwapped += SetTarget;
	}

	public override void RemoveEventListeners() {
		SceneController.timeSwapped -= SetTarget;
	}

	void Update() {
		if (cutSceneMovement) {
			if (targetPos != default(Vector3))
				MoveTowards (targetPos);
		} else {
			if (target != null)
				cam.transform.position = target.transform.position + new Vector3(0,0,-10);
		}
	}

	void SetTarget(object sender, PlayerSwitchEventArgs e) {
		target = PlayerController.activeCharacter;
	}

	public static void SetTargetPos(Vector3 pos) {
		targetPos = pos;
		if (pos == null)
			cutSceneMovement = false;
		else {
			cutSceneMovement = true;
			vecToTargetPos = (pos - cam.transform.position).normalized;
		}
	}

	void MoveTowards(Vector3 pos) {
		float dist = Vector3.Distance(targetPos,cam.transform.position);
		cam.transform.Translate (Mathf.Min (Time.deltaTime * camSpeed, dist) * vecToTargetPos);
		if (CameraAtPos (targetPos)) {
			targetPos = default(Vector3);
		}
	}

	public static bool CameraAtPos(Vector3 pos) {
		return Vector3.Distance (cam.transform.position, pos) <= .001f;
	}
}
