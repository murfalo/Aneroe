using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {

	public List<Character> characters;
	Character activeCharacter;
	int characterIndex;
	CameraController camera;

	// Keycodes for direction inputs
	public KeyCode up, right, down, left;
	// Ordered by convention used in Character class
	KeyCode[] directions;

	// Keycodes for other commands
	// Parry is alternate + attack
	public KeyCode alternate, switchCharacter, attack, interact;

	void Awake () {
		camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController>();
		characterIndex = 0;
		activeCharacter = characters [0];
		directions = new KeyCode[4] { up, right, down, left }; 
	}

	void Start() {
		camera.SetTarget (activeCharacter);
	}
	
	void Update () {
		if (activeCharacter.CanAct ()) {
			// Inputs prioritized as such (by order of check):
			// Attacking, Walking, Switching character

			// See if a direction was input and log it
			bool dirChosen = false;
			for (int i = 0; i < directions.Length; i++) { 
				if (Input.GetKey (directions [i])) {
					dirChosen = true;
					activeCharacter.SetDirection (i + 1);
					break;
				}
			}

			if (Input.GetKey (attack)) {
				if (Input.GetKey (alternate))
					activeCharacter.SetBlocking ();
				else
					activeCharacter.SetAttacking ();
			} else if (dirChosen) {
				activeCharacter.StartWalk ();
			} else if (Input.GetKeyDown (switchCharacter)) {
				activeCharacter = characters [++characterIndex];
			}
		}
	}
}
