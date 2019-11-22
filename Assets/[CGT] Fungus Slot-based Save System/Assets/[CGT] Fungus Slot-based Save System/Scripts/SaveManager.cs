using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Fungus;

namespace CGTUnity.Fungus.SaveSystem
{
    /// <summary>
    /// The main interface for creating, loading, and deleting saves.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        
        #region Fields
        // Most of the functionality here is passed off to the following submodules. Much of what 
        // this does without passing the job is react to that the submodules do, hence the 
        // stuff in the event-listener region.
        [SerializeField] protected SaveWriter saveWriter;
        [SerializeField] protected SaveReader saveReader;
        [SerializeField] protected GameLoader gameLoader;
        [SerializeField] protected GameSaver gameSaver;
        protected List<GameSaveData> gameSaves =                    new List<GameSaveData>();
        protected List<GameSaveData> unwrittenSaves =               new List<GameSaveData>();
        protected Dictionary<string, GameSaveData> writtenSaves =   new Dictionary<string, GameSaveData>();
        // ^ Keeping track of what's written or unwritten helps optimize the save-writing 
        // and save-deleting processes.

        #endregion

        #region Properties
        protected virtual string SaveDirectory
        {
            // By default, we're using a save directory relative to the game's launcher for
            // player convenience.
            get                                                     { return Application.dataPath + "/saveData/"; }
        }
        #endregion

        #region Methods
        
        #region MonoBehaviour Standard
        protected virtual void Awake()
        {
            // Get the necessary components
            if (gameLoader == null) gameLoader =    FindObjectOfType<GameLoader>();
            if (gameSaver == null) gameSaver =      FindObjectOfType<GameSaver>();

            if (!Directory.Exists(SaveDirectory))
                Directory.CreateDirectory(SaveDirectory);

            ListenForEvents();
        }

        protected virtual void Start()
        {
            // So other objects (like the SaveSlotManager) can be ready to listen for the save-reading
            saveReader.ReadAllFromDisk(SaveDirectory);
        }

        protected virtual void OnDestroy()
        {
            UnlistenForEvents();
        }

        #endregion

        #region Event Listeners
        protected virtual void OnGameSaveWritten(GameSaveData saveData, string filePath, string fileName)
        {
            unwrittenSaves.Remove(saveData);
            writtenSaves[fileName] =                    saveData;
        }

        protected virtual void OnGameSaveRead(GameSaveData saveData, string filePath, string fileName)
        {
            writtenSaves[fileName] =                    saveData;
            if (!gameSaves.Contains(saveData))
                gameSaves.Add(saveData);
        }

        protected virtual void OnGameSaveErased(GameSaveData saveData, string filePath, string fileName)
        {
            gameSaves.Remove(saveData);
            unwrittenSaves.Remove(saveData);
            writtenSaves[fileName] =                    null;
        }

        #endregion

        #region Save-writing

        /// <summary>
        /// Writes all the unwritten saves this is keeping track of to disk.
        /// </summary>
        public virtual void WriteSavesToDisk()
        {
            saveWriter.WriteAllToDisk(unwrittenSaves, SaveDirectory);
        }

        /// <summary>
        /// Writes the passed save data to disk.
        /// </summary>
        public virtual void WriteSaveToDisk(GameSaveData saveData)
        {
            saveWriter.WriteOneToDisk(saveData, SaveDirectory);
        }

        #endregion

        #region Save-creation and registration

        /// <summary>
        /// Creates and registers new save data with the passed slot's number, then writing it to disk
        /// if set to do so. Save replacement may happen depending on the aformentioned number.
        /// </summary>
        public virtual bool AddSave(SaveSlot slot, bool writeToDisk = true)
        {
            if (slot == null)
                throw new System.NullReferenceException("Cannot register a save with a null slot's number.");

            return AddSave(slot.Number, writeToDisk);
        }

        /// <summary>
        /// Creates and registers new save data with the passed slot number, then writing it to disk
        /// if set to do so. Save replacement may happen depending on the aformentioned number.
        /// </summary>
        public virtual bool AddSave(int slotNumber, bool writeToDisk = true)
        {
            var newSaveData =                       gameSaver.CreateSave(slotNumber);
            return AddSave(newSaveData, writeToDisk);
        }

        /// <summary>
        /// Adds a save to the manager. If the passed save shares a number with one it's already
        /// keeping track of, the old one is replaced with the new one.
        /// In which case, the new one will be written to disk regardless of the 
        /// second argument.
        /// </summary>
        public virtual bool AddSave(GameSaveData newSave, bool writeToDisk = true)
        {
            if (newSave == null)
                return false;

            // See if any save-replacing will happen
            var saveWasReplaced =                 false;

            for (int i = 0; i < gameSaves.Count; i++)
            {
                var oldSave =                       gameSaves[i];
                if (oldSave.SlotNumber == newSave.SlotNumber) // Yes, it will!
                {
                    ReplaceSave(oldSave, newSave);
                    saveWasReplaced =             true;
                    break;
                }
            }

            if (!saveWasReplaced) // Register it normally.
            {
                if (!gameSaves.Contains(newSave))
                    gameSaves.Add(newSave);
                unwrittenSaves.Add(newSave);

                // Write if appropriate.
                if (writeToDisk)
                    WriteSaveToDisk(newSave);
            }

            return true;
        }

        #endregion

        #region Save-erasing
        /// <summary>
        /// Erases the save data with the passed slot number from disk.
        /// </summary>
        public virtual bool EraseSave(int slotNumber)
        {
            for (int i = 0; i < gameSaves.Count; i++)
                if (gameSaves[i].SlotNumber == slotNumber)
                    return EraseSave(gameSaves[i]);
            
            return false;
 
        }

        /// <summary>
        /// Erases the save data linked to the passed slot.
        /// </summary>
        public virtual bool EraseSave(SaveSlot slot)
        {
            if (slot == null)
            {
                Debug.Log("Cannot erase save of a null slot.");
                return false;
            }

            var saveData =                  slot.SaveData;

            if (saveData == null)
            {
                Debug.Log(slot.name + " has no save data to delete.");
                return false;
            }

            return EraseSave(saveData);
        }

        /// <summary>
        /// Erases the file the passed save data was written to from disk.
        /// </summary>
        public virtual bool EraseSave(GameSaveData saveData)
        {
            // Get the file name associated the save data was written into, and use that to delete 
            // it from the save directory.
            // Using foreach because key-value collections are unindexable.
            var eraseSuccessful =               false;

            foreach (var fileName in writtenSaves.Keys)
            {
                if (writtenSaves[fileName] == saveData)
                {
                    var filePath =              SaveDirectory + fileName;
                    File.Delete(filePath);
                    eraseSuccessful =           true;
                    Signals.GameSaveErased.Invoke(saveData, filePath, fileName);
                    break;
                }
            }

            return eraseSuccessful;
        }
        #endregion

        #region Save-loading
        // Ultimately, the loading is always passed off to the GameLoader.

        /// <summary>
        /// Loads a save with the passed slot number. Returns true if successful, false otherwise.
        /// </summary>
        public virtual bool LoadSave(int slotNumber)
        {
            for (int i = 0; i < gameSaves.Count; i++)
            {
                var save =                      gameSaves[i];
                if (save.SlotNumber == slotNumber)
                    return LoadSave(save);
            }

            return false;
        }

        /// <summary>
        /// Loads the save data assigned to the passed slot. Returns true if successful, false
        /// otherwise.
        /// </summary>
        public virtual bool LoadSave(SaveSlot slot)
        {
            // Validate input.
            if (slot == null)
                throw new System.NullReferenceException("Cannot load save data from a null slot.");

            if (slot.SaveData == null)
            {
                Debug.LogWarning("Cannot load save data from " + slot.name + "; it has no save data assigned to it.");
                return false;
            }
            
            return LoadSave(slot.SaveData);
        }

        public virtual bool LoadSave(GameSaveData saveData)
        {
            return gameLoader.Load(saveData);
        }

        #endregion

        #region Helpers
        // When it comes to saves being read or written, this manager only cares when it's the specified
        // save readers and writers doing it.
        protected virtual void ListenForEvents()
        {
            saveWriter.GameSaveWritten +=               OnGameSaveWritten;
            saveReader.GameSaveRead +=                  OnGameSaveRead;
            Signals.GameSaveErased +=         OnGameSaveErased;
        }

        protected virtual void UnlistenForEvents()
        {
            saveWriter.GameSaveWritten -=               OnGameSaveWritten;
            saveReader.GameSaveRead -=                  OnGameSaveRead;
            Signals.GameSaveErased -=         OnGameSaveErased;
        }

        /// <summary>
        /// Both saves are assumed to have the same slot number.
        /// </summary>
        protected virtual void ReplaceSave(GameSaveData oldSave, GameSaveData newSave)
        {
            gameSaves.Remove(oldSave);
            gameSaves.Add(newSave);

            var writeNewSave = false;

            // If the dict had the old save data, write the new one to replace it. Using foreach due to
            // dict limitations.
            foreach (var fileName in writtenSaves.Keys)
            {
                if (writtenSaves[fileName] == oldSave)
                {
                    writeNewSave =              true;
                    break;
                }
            }

            if (writeNewSave)
            {
                WriteSaveToDisk(newSave);
            }
            else
            {
                unwrittenSaves.Remove(oldSave);
                unwrittenSaves.Add(newSave);
            }
        }
        #endregion
        #endregion

    }
}