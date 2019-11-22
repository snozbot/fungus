using UnityEngine;
using CGTUnity.Fungus.NarrativeLogSystem;
using CGTUnity.Fungus.SaveSystem;

/// <summary>
/// To help test saving and loading the state of a NarrativeLog.
/// </summary>
public class NarrativeLogSaveTester : MonoBehaviour
{
    [SerializeField] NarrativeLog narrativeLog =    null;
    [SerializeField] NarrativeLogSaver saver =      null;
    [SerializeField] NarrativeLogLoader loader =    null;
    NarrativeLogData saveData =                     null;

    public void Save()
    {
        saveData =                                  saver.CreateSave();
    }

    public void Load()
    {
        loader.Load(saveData);
    }


}
