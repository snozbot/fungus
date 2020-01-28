using Fungus.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Helps test saving and loading games, proving you can use this system's API to go UI-less.
/// </summary>
public class SaveAndLoadTester : MonoBehaviour
{
    [SerializeField] private string saveKey = "gameSaveData";
    private GameSaver gameSaver;
    private GameLoader gameLoader;

    private GameSaveData saveData;

    private void Awake()
    {
        gameLoader = FindObjectOfType<GameLoader>();
        gameSaver = FindObjectOfType<GameSaver>();
    }

    public void SaveGame()
    {
        saveData = gameSaver.CreateSave();

        // Write the save data to PlayerPrefs
        var jsonSave = JsonUtility.ToJson(saveData);
        PlayerPrefs.SetString(saveKey, jsonSave);
    }

    public void LoadGame()
    {
        // Load the data from PlayerPrefs
        var jsonSave = PlayerPrefs.GetString(saveKey);
        saveData = JsonUtility.FromJson<GameSaveData>(jsonSave);
        gameLoader.Load(saveData);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        //return;
        if (loadMode == LoadSceneMode.Single)
        {
            // Get the gameLoader to load
            SceneManager.sceneLoaded -= OnSceneLoaded;
            gameLoader = FindObjectOfType<GameLoader>();
            gameLoader.LoadState(saveData);
        }
    }
}