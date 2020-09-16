// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [AddComponentMenu("")]
    /// <summary>
    /// Manages the Save History (a list of Save Points) and provides a set of operations for saving and loading games.
    /// </summary>
    public class SaveManager : MonoBehaviour
    {
        public enum SaveType
        {
            Auto,
            Slot,
            Any,
        }
        public SaveFileManager SaveFileManager { get; private set; }

        public void Awake()
        {
            SaveFileManager = new SaveFileManager();

            IsSaveLoading = false;
            IsSavingAllowed = true;
            IsLoadingAllowed = true;

            StartScene = SceneManager.GetActiveScene().name;

            //listen to profile changes
            //and reset when it changes
            SaveManagerSignals.OnSavePreLoad += SaveManagerSignals_OnSavePreLoad;
            SaveManagerSignals.OnSaveLoaded += SaveManagerSignals_OnSaveLoaded;
            UserProfileManagerSignals.OnUserProfileChanged += UserProfileManagerSignals_OnUserProfileChanged;
        }

        private void UserProfileManagerSignals_OnUserProfileChanged()
        {
            RefreshMetas();
        }

        public void RefreshMetas()
        {
            SaveFileManager.PopulateSaveMetas();


            //TODO look at the settings and ensure we have saves in correct order for user saves and put dumbies in where we don't
            var userSaves = CollectUserSaves();

            for (int i = 0; i < NumberOfSlotSaves; i++)
            {
                if (userSaves.Find(x => x.saveName.EndsWith(i.ToString())) == null) //even with leading zeros ends with should match
                {
                    SaveFileManager.SaveMetas.Add(new SaveGameMetaData() { saveName = FungusConstants.SlotSavePrefix + i.ToString() });
                }
            }
        }

        private void SaveManagerSignals_OnSaveLoaded(string savePointKey)
        {
            if (lastSaveName == savePointKey)
                StartCoroutine(DelaySetNotLoading());
        }

        private void SaveManagerSignals_OnSavePreLoad(string savePointKey)
        {
            if (lastSaveName == savePointKey)
                IsSaveLoading = true;
        }

        /// <summary>
        /// Set during SaveManager loading, intended to be used by any class that wants conditional logic
        /// for a 'normal' level load vs one caused by a the save manager.
        /// </summary>
        public bool IsSaveLoading { get; protected set; }

        /// <summary>
        /// The scene that should be loaded when restarting a game.
        /// </summary>
        public string StartScene { get; set; }

        /// <summary>
        /// If false, calls to Save will be immediately short circuited. Intended for user to prevent saving
        /// during gameplay sections that are either undesirable or not safe to save within.
        /// </summary>
        public virtual bool IsSavingAllowed
        {
            get { return _isSavingAllowed; }
            set { _isSavingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        protected bool _isSavingAllowed = true;

        /// <summary>
        /// If false, calls to Load will be immediately short circuited. Intended for user to prevent loading
        /// during gameplay sections that are either undesirable or for somereason unsafe to do so.
        /// </summary>
        public virtual bool IsLoadingAllowed
        {
            get { return _isLoadingAllowed; }
            set { _isLoadingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        protected bool _isLoadingAllowed = true;
        private string lastSaveName;

        public virtual void SaveAuto(string saveName, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return;

            SaveFileManager.Save(FungusConstants.AutoSavePrefix + saveName, savePointDescription, FungusConstants.AutoSavePrefix);

            //if we limit autos and it is an auto, are there now to many, delete oldest until not over limit
            if (NumberOfAutoSaves >= 0)
            {
                var autoSaves = CollectAutoSaves();

                for (int i = 0; i < autoSaves.Count - NumberOfAutoSaves; i++)
                {
                    DeleteSave(autoSaves[i],false);
                }
            }
        }

        public virtual bool SaveSlot(int index, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return false;

            if (index < 0 || index >= NumberOfSlotSaves)
                return false;

            string formatString = "D" + NumberOfSlotSaves.ToString().Length.ToString();

            SaveFileManager.Save(FungusConstants.SlotSavePrefix + index.ToString(formatString), savePointDescription, FungusConstants.SlotSavePrefix);
            return true;
        }

        public virtual void SaveCustom(string saveName, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return;

            SaveFileManager.Save(saveName, savePointDescription, string.Empty);
        }

        public virtual void ReplaceSave(SaveGameMetaData meta, string newSavePointDescription)
        {
            if (!IsSavingAllowed || meta.saveName.StartsWith(FungusConstants.AutoSavePrefix))
                return;

            DeleteSave(meta);

            SaveFileManager.Save(
                meta.saveName,
                newSavePointDescription,
                meta.saveName.StartsWith(FungusConstants.SlotSavePrefix) ? FungusConstants.SlotSavePrefix : string.Empty);
        }

        public virtual bool Load(SaveGameMetaData meta)
        {
            if (!IsLoadingAllowed)
                return false;

            lastSaveName = meta.saveName;

            return SaveFileManager.Load(meta);
        }

        public virtual bool LoadSlot(int index)
        {
            var saveIndex = SaveFileManager.SaveNameToIndex(FungusConstants.SlotSavePrefix + index.ToString());

            if (saveIndex < 0)
                return false;

            return SaveFileManager.Load(SaveFileManager.SaveMetas[saveIndex]);
        }

        /// <summary>
        /// Reload the starting scene, without setting the loading flag. If requested, can delete all saves on
        /// the current profile.
        /// </summary>
        /// <param name="deleteSaves"></param>
        /// <returns></returns>
        public virtual bool Restart(bool deleteSaves)
        {
            if (string.IsNullOrEmpty(StartScene))
            {
                Debug.LogError("No start scene specified");
                return false;
            }

            // Reset the Save History for a new game
            if (deleteSaves)
            {
                SaveFileManager.DeleteAllSaves();
                SaveFileManager.PopulateSaveMetas();
            }

            SaveManagerSignals.DoSaveReset();
            SceneManager.LoadScene(StartScene);
            return true;
        }

        /// <summary>
        /// Used to allow full enable, start, update to run before turning off our loading flag.
        /// </summary>
        /// <returns></returns>
        private System.Collections.IEnumerator DelaySetNotLoading()
        {
            yield return new WaitForEndOfFrame();
            IsSaveLoading = false;
            lastSaveName = string.Empty;
        }

        /// <summary>
        /// Return the most recently written save regardless of type of save.
        /// </summary>
        /// <returns></returns>
        public virtual SaveGameMetaData GetMostRecentSave()
        {
            if (SaveFileManager.SaveMetas.Count > 0)
            {
                var newestSaveTime = SaveFileManager.SaveMetas.Max(x => x.lastWritten);

                return SaveFileManager.SaveMetas.FirstOrDefault(x => x.lastWritten == newestSaveTime);
            }

            return null;
        }

        /// <summary>
        /// Gather and return all Auto saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SaveGameMetaData> CollectAutoSaves()
        {
            return SaveFileManager.SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.AutoSavePrefix))
                .OrderBy(x => x.lastWritten.Ticks).ToList();
        }

        /// <summary>
        /// Gather and return all User (slot) saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SaveGameMetaData> CollectUserSaves()
        {
            return SaveFileManager.SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.SlotSavePrefix))
                .OrderBy(x => System.Convert.ToInt32(x.saveName.Substring(FungusConstants.SlotSavePrefix.Length))).ToList();
        }

        public void DeleteSave(SaveGameMetaData meta, bool replaceIfSlot = true)
        {
            SaveFileManager.DeleteSave(SaveFileManager.SaveMetas.IndexOf(meta));

            if (replaceIfSlot && meta.saveName.StartsWith(FungusConstants.SlotSavePrefix))
            {
                SaveFileManager.SaveMetas.Add(new SaveGameMetaData() { saveName = meta.saveName });
            }
        }

        protected int numAutoSaves = 1, numSlotSaves = 0;

        public int NumberOfSlotSaves { get { return numSlotSaves; } }

        public int NumberOfAutoSaves { get { return numAutoSaves; } }

        /// <summary>
        /// Determines the number of saves expected and maintained for the current profile by the save manager.
        ///
        /// PopulatesSaveMetas when called.
        /// </summary>
        /// <param name="numAutos">Max auto saves, after which, the oldest will be removed</param>
        /// <param name="numUser">Slots for user saves that will be maintained</param>
        public void ConfigureSaveNumber(int numAutos, int numUser)
        {
            numAutoSaves = numAutos;
            numSlotSaves = numUser;
            RefreshMetas();
        }
    }
}
