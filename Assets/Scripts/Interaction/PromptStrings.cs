using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PromptStrings {

	public static Dictionary<string, string> prompts = new Dictionary<string, string> () {
		{ "introWasd", "Press W, A, S, and D to move around" },
		{ "introMouse", "Press the mouse to use your weapon. Left click to attack, right click to block" }
	};
}
