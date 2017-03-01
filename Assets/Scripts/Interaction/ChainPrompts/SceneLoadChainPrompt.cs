using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SceneLoadPrompt : ChainPrompt
{

	public float fadeDuration;
	public bool fadeOut;

	Color oldColor;

	public override void BeginPrompt() {
		if (promptIndex != -1)
			return;
		promptIndex++;
		WaitOnFade (false);
	}

	public override void ContinuePrompt() {
		if (promptIndex < stringPrompts.Length - 1) {
			promptIndex++;
			WaitOnFade (true);
		} else {
			PromptController.textPrompted (this, null);
		}
	}

	IEnumerator WaitOnFade(bool overridePrompt) {
		// If fading out, opaque to cover screen. Transperant otherwise
		int fadeDirection = fadeOut ? 1 : -1;
		Image camFader = UIController.CamFader;
		oldColor = camFader.color;
		while (UIController.CamFader.color.a != (fadeOut ? 1 : 0)) {
			// Forced between 0 and 1, variable in fade direction
			float newAlpha = Mathf.Max (0, Mathf.Min (1, camFader.color.a + Time.fixedDeltaTime / fadeDirection / fadeDuration));
			camFader.color = new Color (oldColor.r, oldColor.g, oldColor.b, newAlpha);
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		SendPromptEvent (overridePrompt);
	}
}

