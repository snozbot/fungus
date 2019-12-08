using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fungus;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// Helps signify where into a game the player has gotten, so that when a GameSaveData is loaded, 
    /// the game can react appropriately.
    /// </summary>
    [CommandInfo("Flow", 
                "Progress Marker", 
                "Marks a point for where a player has gotten. Helps with reacting to GameSaveData being loaded.")]
    public class ProgressMarker : Command
    {
        [SerializeField] string key =                   null;
        public virtual string Key
        {
                                                        get { return key; }
        }

        public static ProgressMarker latestExecuted     { get; set; }

        public override void OnEnter()
        {
            base.OnEnter();
            latestExecuted =                        this;
            Continue();
        }

        /// <summary>
        /// Searches the scene for a ProgressMarker with the passed key, returning it if found.
        /// Null otherwise.
        /// </summary>
        public static ProgressMarker FindWithKey(string key)
        {
            var markers =                           FindObjectsOfType<ProgressMarker>();
            for (int i = 0; i < markers.Length; i++)
                if (markers[i].key == key)
                    return markers[i];

            return null;
        }
    }
}