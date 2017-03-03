using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public string sceneName;

	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<PlayerEntity> () != null) {
			SceneController.AddScene (sceneName);
		}
	}
}

