using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : BaseController
{

    public bool cutToStartScene;
    public string startScene;

    /// <summary>Camera on the currently selected scene.</summary>
    private CameraController cam;

    public static event EventHandler<EventArgs> timeSwapped;

    Scene oldScene;

    void Awake()
    {
        oldScene = SceneManager.GetActiveScene();
        // Take every gameobject in base scene hierarchy
        //foreach (GameObject obj in DontDestroys) {
        //	DontDestroyOnLoad (obj);
        //}
    }

    /// <section>Grab the correct camera object and set it's target to the active character.</section>
    public override void ExternalSetup()
    {
        cam = GameObject.Find("Control").GetComponent<CameraController>();
        cam.SetTarget(PlayerController.activeCharacter);
    }

    void Start()
    {
        // Load correct scene
        if (cutToStartScene)
        {
            if (!SceneManager.GetActiveScene().name.Equals(startScene))
            {
                SceneManager.LoadScene(startScene, LoadSceneMode.Additive);
                //print (SceneManager.GetActiveScene ().name);
                //print (SceneManager.GetSceneByName (startScene).name);
                //SceneManager.SetActiveScene (SceneManager.GetSceneByName (startScene));
                SceneManager.sceneLoaded += LoadedScene;
            }
        }
    }

    void Update()
    {
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

        SceneManager.MergeScenes(oldScene, newScene);
        oldScene = newScene;
        if (timeSwapped != null)
            timeSwapped(this, new EventArgs());

        foreach (BaseController obj in gameObject.GetComponents<BaseController>())
        {
            obj.InternalSetup();
        }
        foreach (BaseController obj in gameObject.GetComponents<BaseController>())
        {
            obj.ExternalSetup();
        }
    }

    public void ReloadBaseScene()
    {
        SceneManager.sceneLoaded -= LoadedScene;
        SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
    }

    public void ChangeActiveCharacter()
    {
        cam.SetTarget(PlayerController.activeCharacter);
        // Add scene loading functionality here
        // timeSwapped(this, new EventArgs());
        Scene newScene = default(Scene);
        // If loaded new scene, do not call time swapped yet
        if (newScene != default(Scene)) return;
        if (timeSwapped != null)
            timeSwapped(this, new EventArgs());
    }
}
