using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	public GameObject[] startMenus;
	public GameObject[] saves;

	public string startScene;

	public static string globalSaveLocation = "/gameData.dat";
	static Hashtable globalSaveData;

	// The name of the scene to be loaded by scene controller
	public static string sceneToLoad;

	const string SCENE_KEY = "_current_scene";

	void Awake()
	{
		// Never destroy the game controller. Never.
		DontDestroyOnLoad (gameObject);

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

		for (int i = 0; i < saves.Length; i++) {
			Text saveText = saves [i].GetComponentInChildren<Text> ();
			string lvlName = "Empty";
			string saveName = "save" + i.ToString () + SCENE_KEY;
			if (globalSaveData.ContainsKey (saveName))
				lvlName = (string)globalSaveData [saveName];
			saveText.text = saveText.text + lvlName;
		}

		OpenWindow (0);
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
		string sceneKey = "save" + index.ToString() + SCENE_KEY;
		if (globalSaveData.ContainsKey (sceneKey)) {
			sceneToLoad = (string)globalSaveData [sceneKey];
		} else {
			globalSaveData.Add (sceneKey, startScene);
			sceneToLoad = startScene;
		}
		SceneManager.LoadScene ("BaseScene", LoadSceneMode.Single);
	}

	// Called by objects in game to save global save data
	public static void SetSaveValue<T>(string key, T value)
	{
		if (!globalSaveData.ContainsKey (key))
			globalSaveData.Add (key, value);
		else
			globalSaveData [key] = value;
	}

	// Called by objects in game to retrieve global save data
	public static void GetSaveValue<T>(string key, out T value)
	{
		value = default(T);
		if (globalSaveData.ContainsKey(key))
			value = (T)globalSaveData[key];
	}

	public static void Save () {
		if (globalSaveData != null) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream sf = File.Create (Application.persistentDataPath + globalSaveLocation);
			bf.Serialize (sf, globalSaveData);
			sf.Close ();
		}
	}

	public void ExitGame() {
		Application.Quit ();
	}
}

