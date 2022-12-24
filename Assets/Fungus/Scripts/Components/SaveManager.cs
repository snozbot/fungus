// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    /// <summary>
    /// Manages the game's saves, is the entry point for creating new saves and loading saves, when Auto or Slot based saves are being used.
    /// Given this is the single entry point for talking to the save and load system it also supports disabling saving as desired. Being
    /// a MonoBeh it also deals with some of the logic related to tracking when a load is occuring across a Unity Scene change.
    /// </summary>
    [AddComponentMenu("")]
    public class SaveManager : MonoBehaviour
    {
        protected bool _isLoadingAllowed = true;

        protected bool _isSavingAllowed = true;

        protected int numAutoSaves = 1, numSlotSaves = 0;

        private string lastSaveName;

        public enum SaveType
        {
            Auto,
            Slot,
            Any,
        }

        /// <summary>
        /// If false, calls to Load will be immediately short circuited. Intended for user to prevent loading
        /// during gameplay sections that are either undesirable or for somereason unsafe to do so.
        /// </summary>
        public bool IsLoadingAllowed
        {
            get { return _isLoadingAllowed; }
            set { _isLoadingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        /// <summary>
        /// Set during SaveManager loading, intended to be used by any class that wants conditional logic
        /// for a 'normal' level load vs one caused by a the save manager.
        /// </summary>
        public bool IsSaveLoading { get; protected set; }

        /// <summary>
        /// If false, calls to Save will be immediately short circuited. Intended for user to prevent saving
        /// during gameplay sections that are either undesirable or not safe to save within.
        /// </summary>
        public bool IsSavingAllowed
        {
            get { return _isSavingAllowed; }
            set { _isSavingAllowed = value; SaveManagerSignals.DoSavingLoadingAllowedChanged(); }
        }

        public int NumberOfAutoSaves { get { return numAutoSaves; } }
        public int NumberOfSlotSaves { get { return numSlotSaves; } }
        public SaveFileManager SaveFileManager { get; private set; }

        /// <summary>
        /// The scene that should be loaded when restarting a game.
        /// </summary>
        public string StartScene { get; set; }

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

        /// <summary>
        /// Gather and return all Auto saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SaveGameMetaData> CollectAutoSaves()
        {
            return SaveFileManager.SaveMetas.Where(x => x.saveName.StartsWith(SaveSysConstants.AutoSavePrefix))
                .OrderBy(x => x.lastWritten.Ticks).ToList();
        }

        /// <summary>
        /// Gather and return all User (slot) saves currently in our meta list.
        /// </summary>
        /// <returns></returns>
        public List<SaveGameMetaData> CollectUserSaves()
        {
            return SaveFileManager.SaveMetas.Where(x => x.saveName.StartsWith(SaveSysConstants.SlotSavePrefix))
                .OrderBy(x => System.Convert.ToInt32(x.saveName.Substring(SaveSysConstants.SlotSavePrefix.Length))).ToList();
        }

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

        public void DeleteSave(SaveGameMetaData meta, bool replaceIfSlot = true)
        {
            SaveFileManager.DeleteSave(SaveFileManager.SaveMetas.IndexOf(meta));

            if (replaceIfSlot && meta.saveName.StartsWith(SaveSysConstants.SlotSavePrefix))
            {
                SaveFileManager.SaveMetas.Add(new SaveGameMetaData() { saveName = meta.saveName });

                SaveManagerSignals.DoSaveMetasRefreshed();
            }
        }

        /// <summary>
        /// Return the most recently written save regardless of type of save.
        /// </summary>
        /// <returns></returns>
        public SaveGameMetaData GetMostRecentSave()
        {
            if (SaveFileManager.SaveMetas.Count > 0)
            {
                var newestSaveTime = SaveFileManager.SaveMetas.Max(x => x.lastWritten);

                return SaveFileManager.SaveMetas.FirstOrDefault(x => x.lastWritten == newestSaveTime);
            }

            return null;
        }

        public bool Load(SaveGameMetaData meta)
        {
            if (!IsLoadingAllowed)
                return false;

            lastSaveName = meta.saveName;

            return SaveFileManager.Load(meta);
        }

        public bool LoadSlot(int index)
        {
            var saveIndex = SaveFileManager.SaveNameToIndex(SaveSysConstants.SlotSavePrefix + index.ToString());

            if (saveIndex < 0)
                return false;

            return SaveFileManager.Load(SaveFileManager.SaveMetas[saveIndex]);
        }

        public void RefreshMetas()
        {
            SaveFileManager.PopulateSaveMetas();

            var userSaves = CollectUserSaves();

            for (int i = 0; i < NumberOfSlotSaves; i++)
            {
                if (userSaves.Find(x => x.saveName.EndsWith(i.ToString())) == null) //even with leading zeros ends with should match
                {
                    SaveFileManager.SaveMetas.Add(new SaveGameMetaData() { saveName = SaveSysConstants.SlotSavePrefix + i.ToString() });
                }
            }

            SaveManagerSignals.DoSaveMetasRefreshed();
        }

        public void ReplaceSave(SaveGameMetaData meta, string newSavePointDescription)
        {
            if (!IsSavingAllowed || meta.saveName.StartsWith(SaveSysConstants.AutoSavePrefix))
                return;

            DeleteSave(meta);

            SaveFileManager.Save(
                meta.saveName,
                newSavePointDescription,
                meta.saveName.StartsWith(SaveSysConstants.SlotSavePrefix) ? SaveSysConstants.SlotSavePrefix : string.Empty);
        }

        /// <summary>
        /// Reload the starting scene, without setting the loading flag. If requested, can delete all saves on
        /// the current profile.
        /// </summary>
        /// <param name="deleteSaves"></param>
        /// <returns></returns>
        public bool Restart(bool deleteSaves)
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

        public void SaveAuto(string saveName, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return;

            SaveFileManager.Save(SaveSysConstants.AutoSavePrefix + saveName, savePointDescription, SaveSysConstants.AutoSavePrefix);

            //if we limit autos and it is an auto, are there now to many, delete oldest until not over limit
            if (NumberOfAutoSaves >= 0)
            {
                var autoSaves = CollectAutoSaves();

                for (int i = 0; i < autoSaves.Count - NumberOfAutoSaves; i++)
                {
                    DeleteSave(autoSaves[i], false);
                }
            }
        }

        public void SaveCustom(string saveName, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return;

            SaveFileManager.Save(saveName, savePointDescription, string.Empty);
        }

        public bool SaveSlot(int index, string savePointDescription)
        {
            if (!IsSavingAllowed)
                return false;

            if (index < 0 || index >= NumberOfSlotSaves)
                return false;

            string formatString = "D" + NumberOfSlotSaves.ToString().Length.ToString();

            SaveFileManager.Save(SaveSysConstants.SlotSavePrefix + index.ToString(formatString), savePointDescription, SaveSysConstants.SlotSavePrefix);
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

        private void UserProfileManagerSignals_OnUserProfileChanged()
        {
            RefreshMetas();
        }
    }
}
