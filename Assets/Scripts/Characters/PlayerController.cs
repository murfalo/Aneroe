using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public List<Entity> characters;
	public static Entity activeCharacter;
	int characterIndex;
	CameraController cam;

	public GameObject inventoryUI;

	// Keycodes for direction inputs
	public KeyCode up, right, down, left;
	// Ordered by convention used in Character class
	KeyCode[] directions;

	// Keycodes for other commands
	// Parry is alternate + attack
	public KeyCode alternate, switchCharacter, attack, interact, inventory;

	public string nextScene;

	void Awake () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController>();
		characterIndex = 0;
		activeCharacter = characters [0];
		directions = new KeyCode[4] { up, right, down, left }; 
	}

	void Start() {
		cam.SetTarget (activeCharacter);
	}
	
	void Update () {
		if (Input.GetKeyDown(inventory))
			inventoryUI.SetActive(!inventoryUI.activeSelf);

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
				//SceneManager.LoadScene (nextScene);
				characterIndex = (characterIndex + 1) % characters.Count;
				activeCharacter = characters [characterIndex];
			}
		}
	}
}
