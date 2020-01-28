using UnityEngine.Events;

//TODO some may need to move into fungus

namespace Fungus.SaveSystem
{
    public static class Signals
    {
        /// <summary>
        /// Invoked when an object subclassing SaveData (other than GameSaveData) is created.
        /// </summary>
        public static UnityAction<SaveData> SaveCreated = delegate { };

        /// <summary>
        /// Invoked when a GameSaveData instance is created.
        /// </summary>
        public static UnityAction<GameSaveData> GameSaveCreated = delegate { };

        /// <summary>
        /// Invoked when GameSaveData is written to disk. Params: saveData, filePath, fileName
        /// </summary>
        public static UnityAction<GameSaveData, string, string> GameSaveWritten = delegate { };

        /// <summary>
        /// Invoked when GameSaveData is erased from disk. Params: saveData, filePath, fileName
        /// </summary>
        public static UnityAction<GameSaveData, string, string> GameSaveErased = delegate { };

        /// <summary>
        /// Invoked when GameSaveData is read from disk. Params: saveData, filePath, fileName
        /// </summary>
        public static UnityAction<GameSaveData, string, string> GameSaveRead = delegate { };

        /// <summary>
        /// Invoked when a save slot changes its fields to represent a SaveData instance (or lack thereof).
        /// </summary>
        public static UnityAction<SaveSlot, GameSaveData> SaveSlotUpdated = delegate { };

        public static UnityAction<SaveSlot> SaveSlotClicked = delegate { };

        /// <summary>
        /// Invoked when GameSaveData is loaded.
        /// </summary>
        public static UnityAction<GameSaveData> GameLoaded = delegate { };
    }
}