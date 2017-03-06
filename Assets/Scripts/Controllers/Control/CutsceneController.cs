using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Cutscenes;
using UnityEngine.UI;
using AneroeInputs;

public class CutsceneController : BaseController
{
	// Can only call coroutines from a monobehaviour inherited object, hence the instance reference
	public static CutsceneController instance;

	public static List<string[]> currentSceneActs;
	static int actIndex;

	public static ActProgress[] actProgress;
	public enum ActProgress {
		NotStarted,
		Started,
		Finished
	}

	public static Dictionary<string, Action<string[]>> sceneActions = new Dictionary<string, Action<string[]>>() 
	{
		{"text", PromptText},
		{"moveToMarker",MoveToMarker},
		{"fade",Fade},
		{"loadScene",LoadScene},
	};

	public override void InternalSetup () {
		instance = this;
		currentSceneActs = new List<string[]> ();
		actIndex = int.MaxValue;
	}

	void Update() {
		if (actIndex < currentSceneActs.Count) {
			if (actProgress [actIndex] == ActProgress.Finished) {
				actIndex++;
				if (actIndex == currentSceneActs.Count) {
					actIndex = int.MaxValue;
					if (InputController.mode <= InputInfo.InputMode.Cutscene)
						InputController.mode = InputInfo.InputMode.Free;
					return;
				}
			}
			if (actProgress [actIndex] == ActProgress.NotStarted) {
				sceneActions [currentSceneActs [actIndex] [0]] (currentSceneActs [actIndex]);
			}
		}
	}

	static void PromptText(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		PromptController.cutsceneTextPrompted (null, new TextPromptEventArgs(null,args [1],-1));
	}

	public static void EndTextPrompt() {
		actProgress [actIndex] = ActProgress.Finished;
		// To clear out text prompt 
		PromptController.cutsceneTextPrompted (null, null);
	}

	static void MoveToMarker(string[] args) {
		actProgress [actIndex] = ActProgress.Finished;
	}

	static void Fade(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		instance.StartCoroutine (WaitOnFade (args[1].Equals("out") ? true : false, float.Parse(args [2])));
	}

	static IEnumerator WaitOnFade(bool fadeOut,float fadeDuration) {
		// If fading out, opaque to cover screen. Transperant otherwise
		int fadeDirection = fadeOut ? 1 : -1;
		Image camFader = UIController.CamFader;
		camFader.gameObject.SetActive (true);
		Color oldColor = camFader.color;
		while (UIController.CamFader.color.a != (fadeOut ? 1 : 0)) {
			// Forced between 0 and 1, variable in fade direction
			float newAlpha = Mathf.Max (0, Mathf.Min (1, camFader.color.a + Time.fixedDeltaTime / fadeDirection / fadeDuration));
			camFader.color = new Color (oldColor.r, oldColor.g, oldColor.b, newAlpha);
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		actProgress [actIndex] = ActProgress.Finished;
	}

	static void LoadScene(string[] args) {
		InputController.mode = InputInfo.InputMode.Loading;
		actProgress [actIndex] = ActProgress.Finished;
		SceneController.LoadSceneAlone (args[1]);
	}

	public static void BeginCutscene(string name) {
		instance.StartCoroutine (WaitToActivateCutscene (name));
	}

	static IEnumerator WaitToActivateCutscene(string name) {
		while (InputController.mode > InputInfo.InputMode.Cutscene)
			yield return new WaitForSeconds (Time.fixedDeltaTime);

		currentSceneActs = CutsceneStrings.scenes [name];
		InputController.mode = InputInfo.InputMode.Cutscene;
		actProgress = new ActProgress[currentSceneActs.Count];
		for (int i = 0; i < actProgress.Length; i++) {
			actProgress [i] = ActProgress.NotStarted;
		}
		actIndex = 0;
	}
}

