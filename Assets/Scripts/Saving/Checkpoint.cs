using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
	void OnTriggerEnter2D(Collider2D col) {
		// Confirm player past through checkpoint
		if (PlayerController.activeCharacter != null && col.name.Equals (PlayerController.activeCharacter.name)) {
			StartCoroutine(SaveAsync ());
			// Disable collider for checkpoint to not trigger again
			GetComponent<Collider2D> ().enabled = false;
		}
	}

	IEnumerator SaveAsync() {
		yield return new WaitForEndOfFrame ();
		GameController.Save ();
	}
}

