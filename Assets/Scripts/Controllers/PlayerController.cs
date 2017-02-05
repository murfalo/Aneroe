using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using AneroeInputs;
using UnityEngine.UI;

public class PlayerController : BaseController {

	public List<GameObject> characterPrefabs;
	public static Entity activeCharacter;

	Entity[] characters;
	int characterIndex;
	CameraController cam;
	string[] directions;

	public override void InternalSetup() {
		GameObject obj = new GameObject();
		obj.name = "PlayerHolder";
		characters = new Entity[characterPrefabs.Count];
		for (int i = 0; i < characterPrefabs.Count; i++) {
			characters [i] = ((GameObject)GameObject.Instantiate (characterPrefabs [i],obj.transform)).GetComponent<Entity> ();
			characters [i].Setup ();
			//print ("character: " + characters [i].name + "  parent:  " + characters [i].transform.parent);
		}
		characterIndex = 0;
		activeCharacter = characters [0];
		directions = new string[4] { "up", "right", "down", "left" };
	}

	public override void ExternalSetup() {
		cam = GameObject.Find ("Control").GetComponent<CameraController>();
		GameObject.Find("Control").GetComponent<InputController>().iEvent.inputed += new InputEventHandler (ReceiveInput);
		cam.SetTarget (activeCharacter);
	}

	public void ReceiveInput(object sender, InputEventArgs e) {
		if (activeCharacter.CanAct ()) {
			// Inputs prioritized as such (by order of check):
			// Attacking, Walking, Switching character

			// See if a direction was input and log it
			bool dirChosen = false;
			for (int i = 0; i < directions.Length; i++) { 
				if (e.IsHeld(directions[i])) {
					dirChosen = true;
					activeCharacter.SetDirection (i + 1);
					break;
				}
			}

			if (e.WasPressed("attack")) {
				if (e.IsHeld("alternate"))
					activeCharacter.SetBlocking ();
				else
					activeCharacter.SetAttacking ();
			} else if (dirChosen) {
				activeCharacter.StartWalk ();
			} else if (e.WasPressed("switch character")) {
				characterIndex = (characterIndex + 1) % characters.Length;
				activeCharacter = characters [characterIndex];
				cam.SetTarget (activeCharacter);
			} else if (e.WasPressed("interact")) {
				activeCharacter.interact ();
			}
		}
	}
}
