using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveController : BaseController
{
    /// <summary>Location to save data in the persistent data path.</summary>
    private const string saveLocation = "/playerInfo.dat";

    /// <summary>Hashtable containing state information that will be saved</summary>
    private static Hashtable saveData;

    /// <summary>Event published when saving is about to occur.</summary>
    public static event EventHandler<EventArgs> playerSaving;

    /// <summary>Event published when loading has completed.</summary>
    public static event EventHandler<EventArgs> playerLoaded;

    /// <summary>Initializes a new hashtable in memory to store save data.</summary>
	public override void InternalSetup()
	{
		if (saveData == null)
			saveData = new Hashtable();
		SceneController.mergedNewScene += LoadByEvent;
    }

	public override void RemoveEventListeners() {
		SceneController.mergedNewScene -= LoadByEvent;
	}

    /// <section>Adds a value associated with key to saveData.</section>
    /// <param name="key">Key to associate value with in saveData.</param>
    /// <param name="value">Value to associate with key in saveData.</param>
    public static void SetValue<T>(string key, T value)
    {
        if (!saveData.ContainsKey(key))
            saveData.Add(key, value);
    }

    /// <section>Gets a value from saveData associated with key.</section>
    /// <param name="key">Key of desired value in saveData.</param>
    /// <param name="value">Value to store data associated with key in saveData.</param>
    public static void GetValue<T>(string key, out T value)
    {
        value = default(T);
        if (saveData.ContainsKey(key))
            value = (T)saveData[key];
    }

	public void LoadByEvent(object sender, EventArgs e) {
		Load ();
	}

    /// <summary>Loads the save data from file into the hashtable in memory.</summary>
    public void Load()
    {
        string path = Application.persistentDataPath + saveLocation;
        if (!File.Exists(path)) return;
        BinaryFormatter bf = new BinaryFormatter();
        FileStream lf = File.Open(path, FileMode.Open);
		try {
        	saveData = (Hashtable)bf.Deserialize(lf);
		} catch {
			lf.Close ();
			File.Delete (path);
			return;
		}
        lf.Close();
        if (playerLoaded != null)
            playerLoaded(this, new EventArgs());
    }

    /// <summary>Saves the hash table from memory to a file.</summary>
    public void Save()
    {
        if (playerSaving != null)
            playerSaving(this, new EventArgs());
        BinaryFormatter bf = new BinaryFormatter();
        FileStream sf = File.Create(Application.persistentDataPath + saveLocation);
        bf.Serialize(sf, saveData);
        sf.Close();
    }
}