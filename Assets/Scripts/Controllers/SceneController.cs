using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : BaseController
{

    public bool cutToStartScene;
    public string startScene;

	public static event EventHandler<PlayerSwitchEventArgs> timeSwapped;
	public static event EventHandler mergedNewScene;

    Scene oldScene;

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
			mergedNewScene (this, new EventArgs ());
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
}

public class PlayerSwitchEventArgs : EventArgs {

	public PlayerSwitchEventArgs(PlayerEntity oldP, PlayerEntity newP) {
		newPlayer = newP;
		oldPlayer = oldP;
	}

	public PlayerEntity oldPlayer;
	public PlayerEntity newPlayer;
}