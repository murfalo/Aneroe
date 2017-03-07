using UnityEngine;
using System.Collections;
using AneroeInputs;

public class CameraController : BaseController {

	// For non-cutscene entity trailing
	Entity target;

	// For cutscene camera movement
	public float camSpeed = 4f;

	static Vector3 targetPos;
	static Vector3 vecToTargetPos;
	static GameObject cam;

	public override void InternalSetup() {
		cam = GameObject.Find ("Main Camera");
	}

	public override void ExternalSetup() {
		SceneController.timeSwapped += SetTarget;
	}

	public override void RemoveEventListeners() {
		SceneController.timeSwapped -= SetTarget;
	}

	void Update() {
		if (InputController.mode == InputInfo.InputMode.Cutscene) {
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
		targetPos = new Vector3(pos.x,pos.y,cam.transform.position.z);
	}

	void MoveTowards(Vector3 pos) {
		vecToTargetPos = (targetPos - cam.transform.position).normalized;
		float dist = Vector3.Distance(targetPos,cam.transform.position);
		cam.transform.position += Mathf.Min (Time.deltaTime * camSpeed, dist) * vecToTargetPos;
		if (Vector3.Distance (cam.transform.position, targetPos) <= .001f) {
			targetPos = default(Vector3);
		}
	}

	public static bool CameraAtTarget() {
		return targetPos == default(Vector3);
	}
}
