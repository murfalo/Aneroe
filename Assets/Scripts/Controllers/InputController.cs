using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AneroeInputs {

	public class InputController : MonoBehaviour {
		// state of game's input limitations
		// Free: if all inputs are allowed
		// UI: if no in game input is allowed
		// Paused: if in the escape menu
		public InputInfo.InputMode mode;
		public InputEventWrapper iEvent;

		public Dictionary<string, string> inputPairings = new Dictionary<string, string> {
			{"up","w"},
			{"left","a"},
			{"right","d"},
			{"down","s"},
			{"inventory","i"},
			{"attack","space"},
			{"alternate","left shift"},
			{"switch character","x"}
		};

		public Dictionary<string, string> axisPairings = new Dictionary<string, string>();

		void Awake() {
			iEvent = new InputEventWrapper ();
		}

		void Start () {
		
		}

		void Update () {
			InputEventArgs e = new InputEventArgs ();
			foreach (KeyValuePair<string,string> pair in inputPairings) {
				if (Input.GetKeyDown (pair.Value)) {
					e.actions.Add (pair.Key, new ActionOnInput (0));
				} else if (Input.GetKey (pair.Value)) {
					e.actions.Add (pair.Key, new ActionOnInput (1));
				} else if (Input.GetKeyUp (pair.Value)) {
					e.actions.Add (pair.Key, new ActionOnInput (2));
				}
			}
			foreach (KeyValuePair<string, string> pair in axisPairings) {
				if (Input.GetAxis (pair.Key) != 0) {
					e.actions.Add (pair.Key, new ActionOnInput (Input.GetAxis (pair.Key)));
				}
			}
			iEvent.RegisterKeys (e);
		}
	}

	public delegate void InputEventHandler(object sender, InputEventArgs e);

	public class InputEventWrapper {

		public event InputEventHandler inputed;

		public void RegisterKeys(InputEventArgs e) {
			inputed (this, e);
		}
	}

	public class InputEventArgs : EventArgs {
		// List of inputs received
		public Dictionary<string,ActionOnInput> actions;

		public InputEventArgs() {
			actions = new Dictionary<string, ActionOnInput> ();
		}


		public bool WasPressed(string name) {
			return actions.ContainsKey (name) && actions [name].typeHeld == 0;
		}

		public bool IsHeld(string name) {
			return actions.ContainsKey (name) && actions [name].typeHeld == 1;
		}

		public bool WasReleased(string name) {
			return actions.ContainsKey (name) && actions [name].typeHeld == 2;
		}

		public float GetAxis(string name) {
			if (actions.ContainsKey (name)) {
				return actions [name].axis;
			}
			// Default axis value meaning not inputed at all
			return 0;
		}
	}

	public struct ActionOnInput {
		// Axis value of input (from -1 to 1)
		public float axis;
		// Whether button was pressed, is held down, or was released
		// 0 for pressed, 1 for held, 2 for released
		public int typeHeld;

		public ActionOnInput(int type) {
			typeHeld = type;
			axis = 0;
		}

		public ActionOnInput(float ax) {
			typeHeld = -1;
			axis = ax;
		}
	}

	public static class InputInfo {
		public enum InputMode {
			Free,
			UI,
			Paused
		}
		// Don't hate me
		// To be used so player can switch inputs during a game
		// This ISN'T possible with Unity's API, so I will build it out.
		// I repeat, don't hate me
		public static string[] inputNames = new string[] {
			"backspace","delete","tab","clear","return","pause",
			"escape","space","up","down","right","left","insert",
			"home","end","page up","page down","f1","f2","f3","f4",
			"f5","f6","f7","f8","f9","f10","f11","f12","f13","f14",
			"f15","0","1","2","3","4","5","6","7","8","9","a","b",
			"c","d","e","f","g","h","i","j","k","l","m","n","o","p",
			"q","r","s","t","u","v","w","x","y","z","numlock",
			"caps lock","scroll lock","right shift","left shift",
			"right ctrl","left ctrl","right alt","left alt"
		};
	}
}

