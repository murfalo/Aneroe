using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AneroeInputs;

public class InputTextPrompt : TextPrompt
{
	public string[] andInputNames;
	public string[] orInputNames;
	public int[] andPromptKeys;
	public int[] orPromptKeys;

	bool[] andsPressed;
	bool andPressed, orPressed;

	protected override void Awake() {
		base.Awake ();
		ResetAndOrTrackers ();
	}

	void ResetAndOrTrackers() {
		if (orInputNames.Length == 0)
			orPressed = true;
		else
			orPressed = false;

		andsPressed = new bool[andInputNames.Length];
		for (int i = 0; i < andInputNames.Length; i++) {
			andsPressed [i] = false;
		}
		andPressed = false;
	}

	public override void CheckToContinue() {
		InputEventArgs e = PromptController.lastInputs;
		if (AllInputsRegistered (e) && AnyInputsRegistered (e)) {
			ResetAndOrTrackers ();
			ContinuePrompt ();
		}
	}

	bool AllInputsRegistered(InputEventArgs e) {
		bool allInputed = true;
		if (e == null) {
			return andPressed;
		}
		for (int i = 0; i < andInputNames.Length; i++) {
			if (e.WasPressed (andInputNames [i]) || andPromptKeys[i] != promptIndex)
				andsPressed [i] = true;
			else if (!andsPressed[i])
				allInputed = false;
		}
		if (allInputed)
			andPressed = true;
		return allInputed;
	}

	bool AnyInputsRegistered(InputEventArgs e) {
		if (orPressed)
			return true;
		for (int i = 0; i < orInputNames.Length; i++) {
			if (e.WasPressed (orInputNames [i]) && orPromptKeys[i] == promptIndex) {
				orPressed = true;
				return true;
			}
		}
		return false;
	}
}

