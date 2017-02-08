using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

	public bool cutToStartScene;
	public string startScene;

	Scene oldScene;

	void Awake() {
		oldScene = SceneManager.GetActiveScene ();
		// Take every gameobject in base scene hierarchy
		//foreach (GameObject obj in DontDestroys) {
		//	DontDestroyOnLoad (obj);
		//}
	}

	void Start () {
		// Load correct scene
		if (cutToStartScene) {
			if (!SceneManager.GetActiveScene ().name.Equals (startScene)) {
				SceneManager.LoadScene (startScene, LoadSceneMode.Additive);
				//print (SceneManager.GetActiveScene ().name);
				//print (SceneManager.GetSceneByName (startScene).name);
				//SceneManager.SetActiveScene (SceneManager.GetSceneByName (startScene));
				SceneManager.sceneLoaded += LoadedScene;
			}
		}
	}

	void Update () {
	}

	public void LoadedScene(Scene newScene, LoadSceneMode sceneMode) {
		//Scene oldScene = SceneManager.GetActiveScene ();
		//Scene newScene = SceneManager.GetSceneByName (newSceneName);
		//print ("Old scene: " + oldScene.name);
		//print ("New scene: " + newScene.name);
		StartCoroutine(WaitToMergeScenes(oldScene, newScene));
	}

	IEnumerator WaitToMergeScenes(Scene oldScene, Scene newScene) {
		while (!SceneManager.GetActiveScene ().Equals (newScene)) {
			SceneManager.SetActiveScene (newScene);
			yield return new WaitForSeconds (Time.fixedDeltaTime);
		}

		SceneManager.MergeScenes (oldScene, newScene);
		oldScene = newScene;

		foreach (BaseController obj in gameObject.GetComponents<BaseController>()) {
			obj.InternalSetup ();
		}		
		foreach (BaseController obj in gameObject.GetComponents<BaseController>()) {
			obj.ExternalSetup ();
		}
	}

	public void ReloadBaseScene() {
		SceneManager.sceneLoaded -= LoadedScene;
		SceneManager.LoadScene ("BaseScene", LoadSceneMode.Single);
	}
}
