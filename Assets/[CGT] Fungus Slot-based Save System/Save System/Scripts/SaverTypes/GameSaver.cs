using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fungus;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// This encodes game objects into save data for a whole playthrough, so their state 
    /// can be restored upon loading the game.
    /// Handles Flowcharts and NarrativeLogs by default.
    /// To extend this to handle other data types, just modify or subclass this component.
    /// </summary>
    public class GameSaver: DataSaver<GameSaveData>, ISaveCreator<GameSaveData>
    {
        protected List<DataSaver> subsavers =       new List<DataSaver>();

        #region Methods
        protected virtual void Awake()
        {
           subsavers.AddRange(GetComponents<DataSaver>());
           subsavers.RemoveAll(saver => saver == this); // This can't be its own subsaver!
        }

        public override IList<SaveDataItem> CreateItems()
        {
            // Create GameSaveData, and encode it into a SaveDataItem
            var gameSave =                          CreateSave();
            var jsonSave =                          JsonUtility.ToJson(gameSave);
            var newItem =                           new SaveDataItem(saveType.Name, jsonSave);

            // The array has only one element, since we only made one GameSaveData
            return new SaveDataItem[1] {newItem};
        }

        /// <summary>
        /// Creates and returns save data for the whole game.
        /// </summary>
        public virtual GameSaveData CreateSave()
        {
            var sceneName =                         SceneManager.GetActiveScene().name;
            var newGameSave =                       new GameSaveData(sceneName, -1);
            EncodeInto(ref newGameSave);
            newGameSave.UpdateTime();
            if (ProgressMarker.latestExecuted != null)
                newGameSave.ProgressMarkerKey =     ProgressMarker.latestExecuted.Key;
            
            // It's common to make VN save file descs be the text that was in the textbox, 
            // at the time of the save being made.
            var description =                       newGameSave.Description;
            var sayDialog =                         SayDialog.ActiveSayDialog;

            if (sayDialog != null || string.IsNullOrEmpty(sayDialog.StoryText))
                description =                       sayDialog.StoryText;
            else
                description =                       newGameSave.LastWritten.ToLongDateString();
           
            newGameSave.Description =               description;
            return newGameSave;
        }

        /// <summary>
        /// Creates and returns save data with the passed slot number.
        /// </summary>
        public virtual GameSaveData CreateSave(int slotNumber)
        {
            var newGameSave =                       CreateSave();
            newGameSave.SlotNumber =                slotNumber;
            return newGameSave;
        }

        /// <summary>
        /// Creates and returns save data for the whole game, set for the passed save slot.
        /// </summary>
        public virtual GameSaveData CreateSave(SaveSlot saveSlot)
        {
            return CreateSave(saveSlot.Number);
        }

        #region Helpers

        protected virtual void EncodeInto(ref GameSaveData saveData)
        {
            for (int i = 0; i < subsavers.Count; i++)
            {
                var subsaver =                      subsavers[i];
                var saveDataItems =                 subsaver.CreateItems();
                saveData.Items.AddRange(saveDataItems);
            }
        }

        #endregion

        #endregion


    }
}