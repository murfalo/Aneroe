using UnityEngine;
using System.Collections;
using AneroeInputs;

public class CutscenePrompt : MonoBehaviour
{
	public string cutsceneName;

	/*
	// Pan camera to next location and then send text prompt event
	IEnumerator WaitForCameraPan(bool overridePrompt) {
		while (!CameraController.CameraAtPos (cameraPosition [promptIndex])) {
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		SendPromptEvent (overridePrompt);
	}
	*/

	protected virtual void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<Entity> () == PlayerController.activeCharacter) {
			GetComponent<Collider2D> ().enabled = false;
			CutsceneController.BeginCutscene (gameObject, cutsceneName);
		}
	}
}

