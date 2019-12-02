using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus.SaveSystem;

/// <summary>
/// Helps test saving and loading games, proving you can use this system's API to go UI-less.
/// </summary>
public class SaveAndLoadTester : MonoBehaviour
{
    [SerializeField] string saveKey =           "gameSaveData";
    GameSaver gameSaver;
    GameLoader gameLoader;

    GameSaveData saveData;

    void Awake()
    {
        gameLoader =                            FindObjectOfType<GameLoader>();
        gameSaver =                             FindObjectOfType<GameSaver>();
    }

    public void SaveGame()
    {
        saveData =                              gameSaver.CreateSave();

        // Write the save data to PlayerPrefs
        var jsonSave =                          JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(saveKey, jsonSave);
    }

    public void LoadGame()
    {
        // Load the data from PlayerPrefs
        var jsonSave =                          PlayerPrefs.GetString(saveKey);
        saveData =                              JsonUtility.FromJson<GameSaveData>(jsonSave);
        gameLoader.Load(saveData);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        //return;
        if (loadMode == LoadSceneMode.Single)
        {
            // Get the gameLoader to load
            SceneManager.sceneLoaded -=     OnSceneLoaded;
            gameLoader =                    FindObjectOfType<GameLoader>();
            gameLoader.LoadState(saveData);
        }
    }
}
