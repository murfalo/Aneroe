using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using AneroeInputs;
using UnityEngine.UI;

public class PlayerController : EntityController {

	public List<GameObject> characterPrefabs;
	public static PlayerEntity activeCharacter;

	PlayerEntity[] characters;
	int characterIndex;
	CameraController cam;
	string[] directions;

	public override void InternalSetup() {
		actionResponses = new Dictionary<string, System.Action<Entity>> () {
			{"die",RestartGame}
		};
		GameObject obj = new GameObject();
		obj.name = "PlayerHolder";
		characters = new PlayerEntity[characterPrefabs.Count];
		for (int i = 0; i < characterPrefabs.Count; i++) {
			characters [i] = ((GameObject)GameObject.Instantiate (characterPrefabs [i],obj.transform)).GetComponent<PlayerEntity> ();
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

	void FixedUpdate() {
		if (activeCharacter != null)
			activeCharacter.DoFixedUpdate ();
	}

	public void ReceiveInput(object sender, InputEventArgs e) {
		// Inputs prioritized as such (by order of check):
		// Attacking, Walking, Switching character

		//activeCharacter.Quicken (e.IsHeld ("quicken"));

		// See if a direction was input and log it
		bool dirChosen = false;
		bool[] dirActive = new bool[4];
		for (int i = 0; i < directions.Length; i++) { 
			if (e.IsHeld (directions [i])) {
				dirChosen = true;
				dirActive [i] = true;
			} else {
				dirActive [i] = false;
			}
		}
		activeCharacter.SetDirections (dirActive);
			
		if (e.IsHeld ("attack")) {
			activeCharacter.TryAttacking ();
		} else if (e.IsHeld ("defend")) {
			activeCharacter.TryBlocking ();
		} else if (e.WasPressed ("interact")) {
			activeCharacter.TryInteracting ();
		} else if (e.WasPressed ("switch character") && activeCharacter.CanSwitchFrom ()) {
			characterIndex = (characterIndex + 1) % characters.Length;
			activeCharacter = characters [characterIndex];
			cam.SetTarget (activeCharacter);
		} else if (e.WasPressed ("switch character") && activeCharacter.CanSwitchFrom ()) {
			characterIndex = (characterIndex + 1) % characters.Length;
			activeCharacter = characters [characterIndex];
			cam.SetTarget (activeCharacter);
		} else if (dirChosen) {
			activeCharacter.TryWalk ();
		}
	}

	void RestartGame(Entity e) {
		GameObject.Find ("Control").GetComponent<SceneController> ().ReloadBaseScene ();
	}
}
