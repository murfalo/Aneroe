using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SceneController : BaseController
{
	public static event EventHandler<PlayerSwitchEventArgs> timeSwapped;
	public static event EventHandler<SceneSwitchEventArgs> mergedNewScene;

    static Scene oldScene;
	// If true, all controllers load from file. Otherwise, only select ones do
	static bool loadStandaloneScene;

    public override void InternalSetup()
    {
		SceneManager.sceneLoaded += LoadedScene;

		// Activate controller load from file as this is the first time loading them
		loadStandaloneScene = true;
    }

	public override void RemoveEventListeners()
	{
		SceneManager.sceneLoaded -= LoadedScene;
	}

	public static void AddScene(string newScene) {
		if (!SceneManager.GetActiveScene ().name.Equals (newScene)) {
			oldScene = SceneManager.GetActiveScene ();
			UIController.ToggleLoadingScreen (true);
			SceneManager.LoadSceneAsync (newScene, LoadSceneMode.Additive);
		}
	}

	public static void RemoveScene(string newScene) {
		Debug.Log (newScene);
		print (SceneManager.GetSceneByName (newScene).buildIndex);
		if (SceneManager.GetSceneByName (newScene).buildIndex != -1) {
			SceneManager.UnloadSceneAsync (newScene);
		}
	}

    public void LoadedScene(Scene newScene, LoadSceneMode sceneMode)
    {
		if (oldScene.IsValid() && !oldScene.name.Equals(newScene.name))
        	StartCoroutine(WaitToMergeScenes(oldScene, newScene));
    }

    IEnumerator WaitToMergeScenes(Scene oldScene, Scene newScene)
    {
        while (!SceneManager.GetActiveScene().Equals(newScene))
        {
            SceneManager.SetActiveScene(newScene);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

		GameObject rootOfNewScene = new GameObject();
		rootOfNewScene.name = newScene.name + "Scene";
		foreach (GameObject rootObject in newScene.GetRootGameObjects()) {
			if (rootObject.name.Equals ("Temp")) {
				// To make sure the rootObject doesn't interfere before it is destroyed
				rootObject.SetActive (false);
				// Destroy it with fire
				Destroy (rootObject);
			} else {
				rootObject.transform.SetParent (rootOfNewScene.transform);
			}
		}
        SceneManager.MergeScenes(oldScene, newScene);
        oldScene = newScene;

		if (mergedNewScene != null)
			mergedNewScene (this, new SceneSwitchEventArgs(rootOfNewScene.name, loadStandaloneScene));
		// No more scenes being merged can be standalone, so
		loadStandaloneScene = false;
		StartCoroutine(WaitUntilNextFrame());
    }

	IEnumerator WaitUntilNextFrame()
	{
		yield return new WaitForSeconds (Time.fixedDeltaTime);
		UIController.ToggleLoadingScreen (false);
	}

    public void ReloadBaseScene()
    {
		foreach (BaseController obj in gameObject.GetComponents<BaseController>()) {
			obj.RemoveEventListeners();
		}
		SceneManager.sceneLoaded -= LoadedScene;
		UIController.ToggleLoadingScreen (true);
        SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
    }

	public static void LoadSceneAlone(string sceneName)
	{
		// Destroy old scene objects
		GameObject[] objs = SceneManager.GetActiveScene().GetRootGameObjects();
		for (int i = 0; i < objs.Length; i++) {
			GameObject obj = objs [i];
			if (obj.name.Contains ("Scene")) {
				Destroy (obj);
			}
		}

		RemoveTempGameObjects ();
		loadStandaloneScene = true;
	
		// Load scene
		SceneController.AddScene (sceneName);
	}

	public static void ChangeActiveCharacter(PlayerEntity oldP, PlayerEntity newP)
	{
		// Add scene loading functionality here
		//Scene newScene = default(Scene);

        if (timeSwapped != null)
			timeSwapped(null, new PlayerSwitchEventArgs(oldP, newP));
    }

	static void RemoveTempGameObjects() {
		Destroy (GameObject.Find ("Items"));
		GameObject newHolder = new GameObject ();
		newHolder.name = "Items";
		Destroy (GameObject.Find ("PlayerHolder"));
	}
}

public class PlayerSwitchEventArgs : EventArgs {

	public PlayerSwitchEventArgs(PlayerEntity oldP, PlayerEntity newP) {
		newPlayer = newP;
		oldPlayer = oldP;
	}

	public PlayerEntity oldPlayer;
	public PlayerEntity newPlayer;
}

public class SceneSwitchEventArgs : EventArgs {

	// Scene name being loaded (used to locate root gameobject for that scene)
	public string newSceneName;
	// Whether controllers need to be loaded in (true if booting up game, false if loading additive scene)
	public bool loadStandaloneScene;

	public bool newPlaythrough;

	public SceneSwitchEventArgs(string sceneName, bool loadS) {
		newSceneName = sceneName;
		loadStandaloneScene = loadS;
	}
}