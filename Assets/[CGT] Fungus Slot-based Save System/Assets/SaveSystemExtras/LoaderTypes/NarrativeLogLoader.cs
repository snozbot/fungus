using UnityEngine;
using CGTUnity.Fungus.NarrativeLogSystem;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// Loads the state of one or more NarrativeLogs.
    /// </summary>
    public class NarrativeLogLoader : SaveLoader<NarrativeLogData>
    {

        public override bool Load(NarrativeLogData logData)
        {
            // Tell the UI to register the entries the passed log data has
            NarrativeLogUI logUI =          GameObject.FindObjectOfType<NarrativeLogUI>();

            if (logUI == null)
            {
                string message =            
                @"Cannot load NarrativeLogData without a 
                NarrativeLogUI component in the scene.";
                Debug.LogError(message);
                return false;
            }

            logUI.SetLogEntries(logData.Entries);
            return true;
        }
    }
}