using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using AneroeInputs;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public List<Entity> characters;
	public static Entity activeCharacter;
	int characterIndex;
	CameraController cam;
	public GameObject inventoryUI;
	InputEventWrapper iEvent;
	string[] directions;
	public KeyCode inventory, interact;
	public string nextScene;

	void Awake () {
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<CameraController>();
		characterIndex = 0;
		activeCharacter = characters [0];
		//directions = new KeyCode[4] { up, right, down, left }; 
		directions = new string[4] { "up", "right", "down", "left" };
	}

	void Start() {
		iEvent = GameObject.Find("Control").GetComponent<InputController>().iEvent;
		iEvent.inputed += new InputEventHandler (ReceiveInput);
		cam.SetTarget (activeCharacter);
	}
	
	void Update () {
		if (Input.GetKeyDown(inventory))
			inventoryUI.SetActive(!inventoryUI.activeSelf);
	}

	public void ReceiveInput(object sender, InputEventArgs e) {
		if (Input.GetKeyDown(inventory))
			inventoryUI.SetActive(!inventoryUI.activeSelf);
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
				if (e.WasPressed("alternate"))
					activeCharacter.SetBlocking ();
				else
					activeCharacter.SetAttacking ();
			} else if (dirChosen) {
				activeCharacter.StartWalk ();
			} else if (e.WasPressed("switch character")) {
				//SceneManager.LoadScene (nextScene);
				characterIndex = (characterIndex + 1) % characters.Count;
				activeCharacter = characters [characterIndex];
			} else if (Input.GetKeyDown (interact)) {
				activeCharacter.interact ();
				cam.SetTarget (activeCharacter);
			}
		}
	}
}
