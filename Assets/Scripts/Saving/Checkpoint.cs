using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col) {
		// Confirm player past through checkpoint
		if (col.name.Equals (PlayerController.activeCharacter.name)) {
			GameController.Save ();
			// Disable collider for checkpoint to not trigger again
			GetComponent<Collider2D> ().enabled = false;
		}
	}
}

