using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : BaseController
{

	// Add event information to saving/loading:
	// Scene name being loaded
	// Whether this is loading/reloading controllers
	//  GameObjects can check if they are loading themselves by comparing root's name to scene name
	// Scene name being saved (and unloaded)
	// Whether this is saving everything

    public bool cutToStartScene;
    public string startScene;

	public static event EventHandler<PlayerSwitchEventArgs> timeSwapped;
	public static event EventHandler<SceneSwitchEventArgs> mergedNewScene;

    Scene oldScene;
	bool loadControl;

    void Awake()
    {
        oldScene = SceneManager.GetActiveScene();
    }

    void Start()
	{
		// Initialize controllers
		foreach (BaseController obj in gameObject.GetComponents<BaseController>())
		{
			obj.InternalSetup();
		}
		foreach (BaseController obj in gameObject.GetComponents<BaseController>())
		{
			obj.ExternalSetup();
		}
		// Activate controller load from file as this is the first time loading them
		loadControl = true;

		SaveController.playerLoaded += RemoveTempGameObjects;

		// Activate initial time swap for start of game
		if (timeSwapped != null)
			timeSwapped(this, new PlayerSwitchEventArgs(null,PlayerController.activeCharacter));
		
		// Load correct scene
        if (cutToStartScene)
        {
            if (!SceneManager.GetActiveScene().name.Equals(startScene))
            {
                SceneManager.LoadScene(startScene, LoadSceneMode.Additive);
                SceneManager.sceneLoaded += LoadedScene;
            }
        }
    }

    public void LoadedScene(Scene newScene, LoadSceneMode sceneMode)
    {
        //Scene oldScene = SceneManager.GetActiveScene ();
        //Scene newScene = SceneManager.GetSceneByName (newSceneName);
        //print ("Old scene: " + oldScene.name);
        //print ("New scene: " + newScene.name);
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
			mergedNewScene (this, new SceneSwitchEventArgs(rootOfNewScene.name, loadControl));
		// Whether this is the first scene loading or not, deactivate loading of controllers from now on
		loadControl = false;
    }

    public void ReloadBaseScene()
    {
		foreach (BaseController obj in gameObject.GetComponents<BaseController>()) {
			obj.RemoveEventListeners();
		}
        SceneManager.sceneLoaded -= LoadedScene;
        SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
    }

	public void ChangeActiveCharacter(PlayerEntity oldP, PlayerEntity newP)
	{
		// Add scene loading functionality here
		//Scene newScene = default(Scene);

        if (timeSwapped != null)
			timeSwapped(this, new PlayerSwitchEventArgs(oldP, newP));
    }

	public void RemoveTempGameObjects(object sender, EventArgs e) {
		Destroy (GameObject.Find ("Items"));
		GameObject newHolder = new GameObject ();
		newHolder.name = "Items";
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
	public bool loadControl;

	public bool loadFirstTime;

	public SceneSwitchEventArgs(string sceneName, bool loadC) {
		newSceneName = sceneName;
		loadControl = loadC;
	}
}