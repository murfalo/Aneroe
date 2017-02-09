using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveController : MonoBehaviour
{
    /// <summary>Location to save data in the persistent data path.</summary>
    private const string saveLocation = "/playerInfo.dat";

    /// <summary>Hashtable containing state information that will be saved</summary>
    private Hashtable saveData;

    /// <summary>Event published when saving is about to occur.</summary>
    public event EventHandler<EventArgs> playerSaving;

    /// <summary>Event published when loading has completed.</summary>
    public event EventHandler<EventArgs> playerLoaded;

    /// <summary>Initializes a new hashtable in memory to store save data.</summary>
    void Start()
    {
        saveData = new Hashtable();
    }

    /// <section>Adds a value associated with key to saveData.</section>
    /// <param name="key">Key to associate value with in saveData.</param>
    /// <param name="value">Value to associate with key in saveData.</param>
    public void SetValue<T>(string key, T value)
    {
        if (!saveData.ContainsKey(key))
            saveData.Add(key, value);
    }

    /// <section>Gets a value from saveData associated with key.</section>
    /// <param name="key">Key of desired value in saveData.</param>
    /// <param name="value">Value to store data associated with key in saveData.</param>
    public void GetValue<T>(string key, out T value)
    {
        value = default(T);
        if (saveData.ContainsKey(key))
            value = (T)saveData[key];
    }

    //// <summary>Loads the save data from file into the hashtable in memory.</summary>
    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream lf = File.Open(Application.persistentDataPath + saveLocation, FileMode.Open);
        saveData = (Hashtable)bf.Deserialize(lf);
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