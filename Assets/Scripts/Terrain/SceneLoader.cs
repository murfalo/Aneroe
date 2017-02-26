using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	public string sceneName;
	SceneController sc;


	// Use this for initialization
	void Start ()
	{
		sc = FindObjectOfType<SceneController> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<PlayerEntity> () != null) {
			sc.addScene (sceneName);
		}
	}
}

