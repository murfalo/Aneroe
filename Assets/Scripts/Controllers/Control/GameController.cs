using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class GameController : BaseController
{
	public GameObject startMenuHolder;
	public GameObject[] startMenus;
	public GameObject[] saves;

	public string startScene;
	public string debugStartScene;

	public static string globalSaveLocation = "/gameData.dat";
	// Top-level hashtable serialized into file
	static Hashtable globalSaveData;
	// Hashtable of current save being used 
	static Hashtable saveData;

	// The name of the scene to be loaded by scene controller
	public static string sceneToLoad;

	/// <summary>Event published when saving is about to occur.</summary>
	public static event EventHandler<EventArgs> fileSaving;

	/// <summary>Event published when file loading has completed.</summary>
	public static event EventHandler<SceneSwitchEventArgs> fileLoaded;

	/// <summary>Event published when player loading has completed.</summary>
	public static event EventHandler<SceneSwitchEventArgs> playerLoaded;

	static bool isNewPlaythrough;
	static string currentPlaythroughName;
	const string SCENE_KEY = "_current_scene";
	const string PLAYTHROUGH_KEY = "_playthrough_data";

	void Awake()
	{
		startMenuHolder.SetActive (true);

		// Initialize controllers (including this one)
		foreach (BaseController obj in gameObject.GetComponents<BaseController>())
		{
			obj.InternalSetup();
		}
		foreach (BaseController obj in gameObject.GetComponents<BaseController>())
		{
			obj.ExternalSetup();
		}

		// Load game data
		string path = Application.persistentDataPath + globalSaveLocation;
		BinaryFormatter bf = new BinaryFormatter ();

		if (File.Exists (path)) {
			FileStream lf = File.Open (path, FileMode.Open);
			try {
				globalSaveData = (Hashtable)bf.Deserialize (lf);
			} catch {
				lf.Close ();
				File.Delete (path);
				return;
			}
			lf.Close ();
		} else {
			globalSaveData = new Hashtable ();
		}
		saveData = new Hashtable ();

		// Update start menu UI elements
		for (int i = 0; i < saves.Length; i++) {
			Text saveText = saves [i].GetComponentInChildren<Text> ();
			string lvlName = "Empty";
			string saveName = "save" + i.ToString () + SCENE_KEY;
			if (globalSaveData.ContainsKey (saveName))
				lvlName = (string)globalSaveData [saveName];
			saveText.text = saveText.text + lvlName;
		}
		OpenWindow (0);

		// If applicable, load debug scene instead of traversing start menu
		if (debugStartScene != "") {
			currentPlaythroughName = "saveDebug";
			isNewPlaythrough = true;
			LoadStandaloneScene (debugStartScene);
		}
	}

	public override void ExternalSetup() 
	{
		SceneController.mergedNewScene += LoadCurrentPlaythrough;
	}

	public override void RemoveEventListeners() {
		SceneController.mergedNewScene -= LoadCurrentPlaythrough;
	}

	public void OpenWindow(int index)
	{
		for (int i = 0; i < startMenus.Length; i++) {
			startMenus [i].SetActive (i == index);
		}
	}

	// Called by start menu UI to begin/load game
	public void LoadGameFor(int index)
	{
		currentPlaythroughName = "save" + index.ToString ();
		string sceneKey = currentPlaythroughName + SCENE_KEY;
		if (globalSaveData.ContainsKey (sceneKey)) {
			isNewPlaythrough = false;
			sceneToLoad = (string)globalSaveData [sceneKey];
		} else {
			isNewPlaythrough = true;
			globalSaveData.Add (sceneKey, startScene);
			sceneToLoad = startScene;
		}
		LoadStandaloneScene(GameController.sceneToLoad);
	}

	void LoadStandaloneScene(string sceneName) {
		startMenuHolder.SetActive (false);
		SceneController.LoadSceneAlone (sceneName);
	}

	public static void SetCurrentSceneInSave(string name)
	{
		globalSaveData[currentPlaythroughName + SCENE_KEY] = name;
	}

	// Called by objects in game to save global save data
	public static void SetGlobalValue<T>(string key, T value)
	{
		if (!globalSaveData.ContainsKey (key))
			globalSaveData.Add (key, value);
		else
			globalSaveData [key] = value;
	}

	// Called by objects in game to retrieve global save data
	public static void GetGlobalValue<T>(string key, out T value)
	{
		value = default(T);
		if (globalSaveData.ContainsKey(key))
			value = (T)globalSaveData[key];
	}

	public static void SetSaveValue<T>(string key, T value)
	{
		if (!saveData.ContainsKey (key))
			saveData.Add (key, value);
		else
			saveData [key] = value;
	}

	public static void GetSaveValue<T>(string key, out T value)
	{
		value = default(T);
		if (saveData.ContainsKey(key))
			value = (T)saveData[key];
	}

	public void LoadCurrentPlaythrough(object sender, SceneSwitchEventArgs e) {
		Load (e);
	}

	// Called by UI button
	public void SaveCurrentPlaythrough() {
		Save ();
	}

	// Called by UI button
	public void LoadCurrentPlaythrough() {
		Load (new SceneSwitchEventArgs ("", true));
	}

	public static void Save () {
		// Send save event to collect saveData
		if (fileSaving != null)
			fileSaving(null, new EventArgs());
		
		// Save current playthrough
		string key = currentPlaythroughName + PLAYTHROUGH_KEY;
		if (!globalSaveData.ContainsKey (key))
			globalSaveData.Add (key, saveData);
		else
			globalSaveData [key] = saveData;
		
		if (globalSaveData != null) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream sf = File.Create (Application.persistentDataPath + globalSaveLocation);
			bf.Serialize (sf, globalSaveData);
			sf.Close ();
		}
	}

	public static void Load(SceneSwitchEventArgs e) {
		string key = currentPlaythroughName + PLAYTHROUGH_KEY;
		if (globalSaveData.ContainsKey (key)) {
			saveData = (Hashtable)globalSaveData [key];
		}

		// Specify whether this is a brand new game or not
		e.newPlaythrough = isNewPlaythrough;

		if (fileLoaded != null)
			fileLoaded(null, e);
		if (playerLoaded != null)
			playerLoaded(null, e);
		// Initial time swap
		SceneController.ChangeActiveCharacter (null, PlayerController.activeCharacter);
	}

	public void ExitGame() {
		Application.Quit ();
	}
}

