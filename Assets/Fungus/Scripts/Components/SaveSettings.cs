using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CreateAssetMenu()]
    public class SaveSettings : ScriptableObject
    {
        [Tooltip("Number of auto-saves to maintain, 0 disables auto save, negative means no limit.")]
        [SerializeField] protected int numberOfAutoSaves = 1;
        public int NumberOfAutoSaves { get { return numberOfAutoSaves; } }

        [Tooltip("Number of save slots allowed, 0 disables manual saves, negative means no limit.")]
        [SerializeField] protected int numberOfUserSaves = 3;
        public int NumberOfUserSaves { get { return numberOfUserSaves; } }

        [Tooltip("Delete the save game data from disk when player restarts the game. Useful for testing, but best switched off for release builds.")]
        [SerializeField] protected bool restartDeletesSave = false;
        public bool RestartDeletesSave { get { return restartDeletesSave; } }
    }
}