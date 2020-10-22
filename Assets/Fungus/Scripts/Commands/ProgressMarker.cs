// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Helps signify where into a game the player has gotten, so that when a GameSaveData is loaded,
    /// the game can react appropriately.
    /// </summary>
    [CommandInfo("Save",
                 "Progress Marker",
                 "Marks a point for where a player has reached. Helps with reacting to GameSaveData being loaded.")]
    public class ProgressMarker : Command
    {
        [SerializeField] protected string customKey = string.Empty;

        public virtual string CustomKey
        {
            get { return customKey; }
        }

        protected static ProgressMarker latestExecuted;

        public static ProgressMarker LatestExecuted
        {
            get { return latestExecuted; }
            set
            {
                SaveManagerSignals.DoProgressMarkerReached(latestExecuted, value);
                latestExecuted = value;
            }
        }

        public static string LastExecutedCustomKey
        {
            get
            {
                return ProgressMarker.LatestExecuted != null ? ProgressMarker.LatestExecuted.CustomKey : string.Empty;
            }
        }

        public override void OnEnter()
        {
            LatestExecuted = this;

            Continue();
        }

        public override string GetSummary()
        {
            return customKey;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        /// <summary>
        /// Searches the scene for a ProgressMarker with the passed key, returning it if found.
        /// Null otherwise.
        /// </summary>
        public static ProgressMarker FindWithKey(string key)
        {
            var markers = FindObjectsOfType<ProgressMarker>();
            for (int i = 0; i < markers.Length; i++)
                if (markers[i].customKey == key)
                    return markers[i];

            return null;
        }

        public override void OnValidate()
        {
            base.OnValidate();

            if (string.IsNullOrEmpty(customKey))
            {
                customKey = GetLocationIdentifier();
            }
        }
    }
}
