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
	static GameObject cutsceneObj;

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
		{"wait", Wait},
		{"turn", Turn},
		{"text", PromptText},
		{"takeSteps",TakeSteps},
		{"wound",Wound},
		{"fade",Fade},
		{"loadScene",LoadScene},
		{"timeSwap",TimeSwap},
		{"camTo",MoveCamera},
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

	static void Wait(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		instance.StartCoroutine (WaitOnNothing (float.Parse (args [1])));
	}

	static IEnumerator WaitOnNothing(float time) {
		float timeElapsed = 0;
		while (timeElapsed < time) {
			timeElapsed += Time.deltaTime;
			yield return new WaitForSeconds (Time.deltaTime);
		}
		actProgress [actIndex] = ActProgress.Finished;
	}

	static void Turn(string[] args) {
		actProgress [actIndex] = ActProgress.Finished;
		PlayerController.activeCharacter.SetDir (int.Parse (args [1]));
	}

	static void Wound(string[] args) {
		actProgress [actIndex] = ActProgress.Finished;
		PlayerController.activeCharacter.Damage (float.Parse (args [1]), 1, .2f, 0);
	}

	static void PromptText(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		string speaker = "Self";
		if (args.Length > 2)
			speaker = args [2];
		PromptController.cutsceneTextPrompted (null, new TextPromptEventArgs(null,args [1],-1,speaker));
	}

	public static void EndTextPrompt() {
		actProgress [actIndex] = ActProgress.Finished;
		// To clear out text prompt 
		PromptController.cutsceneTextPrompted (null, null);
	}

	static void TakeSteps(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		PlayerController.activeCharacter.SetDir (int.Parse (args [2]));
		instance.StartCoroutine (TakeAllSteps (int.Parse (args [1])));
	}

	static IEnumerator TakeAllSteps(int num) {
		int numTries;
		for (int i = 0; i < num; i++) {
			numTries = 3;
			while (numTries > 0 && PlayerController.activeCharacter.GetState () != Entity.CharacterState.Walking) {
				PlayerController.activeCharacter.TryWalk ();
				yield return new WaitForSeconds (Time.deltaTime);
				numTries--;
			}
			if (numTries == 0) {
				yield break;
			}
			while (PlayerController.activeCharacter.GetState () != Entity.CharacterState.Still) {
				yield return new WaitForSeconds (Time.deltaTime);
			}
		}
		actProgress [actIndex] = ActProgress.Finished;
	}

	static void Fade(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		instance.StartCoroutine (WaitOnFade (args[1].Equals("out") ? true : false, float.Parse(args [2])));
	}

	static IEnumerator WaitOnFade(bool fadeOut, float fadeDuration) {
		// If fading out, opaque to cover screen. Transperant otherwise
		int fadeDirection = fadeOut ? 1 : -1;
		Image camFader = UIController.CamFader;
		camFader.gameObject.SetActive (true);
		Color oldColor = camFader.color;
		while (Mathf.Abs(UIController.CamFader.color.a - (fadeOut ? 1 : 0)) > .001) {
			// Forced between 0 and 1, variable in fade direction
			float newAlpha = Mathf.Max (0, Mathf.Min (1, camFader.color.a + Time.fixedDeltaTime / fadeDirection / fadeDuration));
			camFader.color = new Color (oldColor.r, oldColor.g, oldColor.b, newAlpha);
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}
		actProgress [actIndex] = ActProgress.Finished;
	}

	static void LoadScene(string[] args) {
		actProgress [actIndex] = ActProgress.Finished;
		InputController.mode = InputInfo.InputMode.Loading;
		SceneController.LoadSceneAlone (args[1]);
		GameController.SetCurrentSceneInSave(args[1]);
	}

	static void TimeSwap(string[] args) {
		actProgress [actIndex] = ActProgress.Finished;
		PlayerController.SwitchActiveCharacters ();
	}

	static void MoveCamera(string[] args) {
		actProgress [actIndex] = ActProgress.Started;
		for (int i = 0; i < cutsceneObj.transform.childCount; i++) {
			Transform child = cutsceneObj.transform.GetChild (i);
			if (child.name.Equals (args [1])) {
				instance.StartCoroutine (WaitForCameraPan (child.transform.position));
				return;
			}
		}
	}

	static IEnumerator WaitForCameraPan(Vector3 pos) {
		CameraController.SetTargetPos (pos);
		while (!CameraController.CameraAtTarget()) {
			yield return new WaitForSeconds (Time.deltaTime);
		}
		actProgress [actIndex] = ActProgress.Finished;
	}

	public static void BeginCutscene(GameObject obj, string name) {
		cutsceneObj = obj;
		instance.StartCoroutine (WaitToActivateCutscene (name));
	}

	static IEnumerator WaitToActivateCutscene(string name) {
		while (InputController.mode > InputInfo.InputMode.Cutscene)
			yield return new WaitForSeconds (Time.deltaTime);

		currentSceneActs = CutsceneStrings.scenes [name];
		InputController.mode = InputInfo.InputMode.Cutscene;
		actProgress = new ActProgress[currentSceneActs.Count];
		for (int i = 0; i < actProgress.Length; i++) {
			actProgress [i] = ActProgress.NotStarted;
		}
		actIndex = 0;
	}
}

