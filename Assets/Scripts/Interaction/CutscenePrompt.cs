using UnityEngine;
using System.Collections;
using AneroeInputs;

public class CutscenePrompt : TextPrompt
{
	public Vector3[] cameraPosition;
	public float[] zoomFactors;

	public ChainPrompt nextPrompt;

	public override void BeginPrompt() {
		if (promptIndex != -1)
			return;
		promptIndex++;
		WaitForCameraPan (false);
	}

	public override void ContinuePrompt() {
		if (promptIndex < stringPrompts.Length - 1) {
			promptIndex++;
			WaitForCameraPan (true);
		} else {
			if (nextPrompt != null)
				nextPrompt.BeginPrompt ();
			else
				PromptController.textPrompted (this, null);
		}
	}

	// Pan camera to next location and then send text prompt event
	IEnumerator WaitForCameraPan(bool overridePrompt) {
		while (!CameraController.CameraAtPos (cameraPosition [promptIndex])) {
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		SendPromptEvent (overridePrompt);
	}
}

