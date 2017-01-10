using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Manages the Save History (a list of Save Points).
    /// </summary>
    public class SaveManager : MonoBehaviour 
    {
        protected static SaveHistory saveHistory = new SaveHistory();

        protected virtual bool ReadSaveHistory(string saveDataKey)
        {
            var historyData = PlayerPrefs.GetString(saveDataKey);
            if (!string.IsNullOrEmpty(historyData))
            {
                var tempSaveHistory = JsonUtility.FromJson<SaveHistory>(historyData);
                if (tempSaveHistory != null)
                {
                    saveHistory = tempSaveHistory;
                    return true;
                }
            }

            return false;
        }

        protected virtual bool WriteSaveHistory(string saveDataKey)
        {
            var historyData = JsonUtility.ToJson(saveHistory, true);
            if (!string.IsNullOrEmpty(historyData))
            {
                PlayerPrefs.SetString(saveDataKey, historyData);
                PlayerPrefs.Save();
                return true;
            }

            return false;
        }

        #region Public members

        /// <summary>
        /// Returns the number of Save Points in the Save History.
        /// </summary>
        public virtual int NumSavePoints { get { return saveHistory.NumSavePoints; } }

        /// <summary>
        /// Returns the current number of rewound Save Points in the Save History.
        /// </summary>
        public virtual int NumRewoundSavePoints { get { return saveHistory.NumRewoundSavePoints; } }

        /// <summary>
        /// Writes the Save History to persistent storage.
        /// </summary>
        public virtual void Save(string saveDataKey)
        {
            WriteSaveHistory(saveDataKey);

            SaveManagerSignals.DoGameSaved(saveDataKey);
        }

        /// <summary>
        /// Loads the Save History from persistent storage.
        /// </summary>
        public void Load(string saveDataKey)
        {
            if (ReadSaveHistory(saveDataKey))
            {
                saveHistory.ClearRewoundSavePoints();
                saveHistory.LoadLatestSavePoint();

                SaveManagerSignals.DoGameLoaded(saveDataKey);
            }
        }

        /// <summary>
        /// Deletes a previously stored Save History from persistent storage.
        /// </summary>
        public void Delete(string saveDataKey)
        {
            PlayerPrefs.DeleteKey(saveDataKey);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Returns true if save data has previously been stored using this key.
        /// </summary>
        public bool SaveDataExists(string saveDataKey)
        {
            return PlayerPrefs.HasKey(saveDataKey);
        }

        /// <summary>
        /// Creates a new Save Point using a key and description, and adds it to the Save History.
        /// </summary>
        public virtual void AddSavePoint(string savePointKey, string savePointDescription)
        {
            saveHistory.AddSavePoint(savePointKey, savePointDescription);

            SaveManagerSignals.DoSavePointAdded(savePointKey, savePointDescription);
        }

        /// <summary>
        /// Rewinds to the previous Save Point in the Save History and loads that Save Point.
        /// </summary>
        public virtual void Rewind()
        {
            if (saveHistory.NumSavePoints > 0)
            {
                // Can't rewind the first save point (new_game)
                if (saveHistory.NumSavePoints > 1)
                {
                    saveHistory.Rewind();
                }

                saveHistory.LoadLatestSavePoint();
            }
        }

        /// <summary>
        /// Fast forwards to the next rewound Save Point in the Save History and loads that Save Point.
        /// </summary>
        public virtual void FastForward()
        {
            if (saveHistory.NumRewoundSavePoints > 0)
            {
                saveHistory.FastForward();
                saveHistory.LoadLatestSavePoint();
            }
        }

        /// <summary>
        /// Deletes all Save Points in the Save History.
        /// </summary>
        public virtual void ClearHistory()
        {
            saveHistory.Clear();
        }
            
        #endregion
    }
}