using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace AneroeInputs {

	public class InputController : BaseController {
		// state of game's input limitations
		// Free: if all inputs are allowed
		// UI: if no in game input is allowed
		// Paused: if in the escape menu
		public static InputInfo.InputMode mode;
		public static InputEventWrapper iEvent;

		public static bool inputingAllowed;

		public Dictionary<string, string> inputPairings = new Dictionary<string, string>() {
			{"w","up"},
			{"a","left"},
			{"d","right"},
			{"s","down"},
			{"e","inventory"},
			{"escape", "mainmenu"},
			{"mouse 0","attack"},
			{"mouse 1","defend"},
			{"left shift","quicken"},
			{"left ctrl","slowen"},
			{"space","switch character"},
			{"z","interact"},
			{"return","return"},
			{"tab","realign"}
		};

		public Dictionary<string, string> axisPairings = new Dictionary<string, string>();

		public override void InternalSetup ()
		{
		    for (var i = 1; i <= InventoryController.ItemsPerRow; i++)
		        inputPairings.Add(i.ToString(), "equip");
			iEvent = new InputEventWrapper ();
			inputingAllowed = false;
			SceneController.mergedNewScene += ActivateInputs;
		}

		public override void RemoveEventListeners() {
			SceneController.mergedNewScene -= ActivateInputs;
		}

		void Update () {
			if (!inputingAllowed)
				return;
			var e = new InputEventArgs ();
			foreach (var pair in inputPairings)
			{
				if (Input.GetKeyDown (pair.Key)) {
					e.actions[pair.Value] = new ActionOnInput (0, pair.Key);
				} else if (Input.GetKey (pair.Key)) {
					e.actions[pair.Value] = new ActionOnInput (1, pair.Key);
				} else if (Input.GetKeyUp (pair.Key)) {
					e.actions[pair.Value] = new ActionOnInput (2, pair.Key);
				}
			}
			foreach (var pair in axisPairings) {
				if (Math.Abs(Input.GetAxis (pair.Value)) > 0.01f) {
					e.actions[pair.Value] = new ActionOnInput (Input.GetAxis (pair.Value), pair.Key);
				}
			}
			if (iEvent != null && e.actions.Count > 0) {
				iEvent.RegisterKeys (e);
			}
		}

		void ActivateInputs(object sender, EventArgs e) {
			inputingAllowed = true;
		}
	}

	public delegate void InputEventHandler(object sender, InputEventArgs e);

	public class InputEventWrapper {

		public event InputEventHandler inputed;

		public void RegisterKeys(InputEventArgs e) {
			if (inputed != null)
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

		public float GetAxis(string name)
		{
		    return actions.ContainsKey (name) ? actions [name].axis : 0;
		    // Default axis value meaning not inputed at all
		}

	    public string GetTrigger(string name)
	    {
	        return actions.ContainsKey(name) ? actions[name].trigger : "";
	    }
	}

	public struct ActionOnInput {
		// Axis value of input (from -1 to 1)
		public float axis;
		// Whether button was pressed, is held down, or was released
		// 0 for pressed, 1 for held, 2 for released
		public int typeHeld;

	    public string trigger;

		public ActionOnInput(int type, string trig) {
			typeHeld = type;
			axis = 0;
		    trigger = trig;
		}

		public ActionOnInput(float ax, string trig) {
			typeHeld = -1;
			axis = ax;
		    trigger = trig;
		}
	}

	public static class InputInfo {
		public enum InputMode {
			Free,
			UI,
			Paused,
			Cutscene
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
			"right ctrl","left ctrl","right alt","left alt","mouse 0","mouse 1"
		};
	}
}

