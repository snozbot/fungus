using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
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

        public virtual int NumSavePoints { get { return saveHistory.NumSavePoints; } }

        public virtual int NumRewoundSavePoints { get { return saveHistory.NumRewoundSavePoints; } }

        public virtual void Save(string saveDataKey)
        {
            WriteSaveHistory(saveDataKey);

            SaveManagerSignals.DoGameSaved(saveDataKey);
        }
 
        public void Load(string saveDataKey)
        {
            if (ReadSaveHistory(saveDataKey))
            {
                saveHistory.LoadLatestSavePoint();

                SaveManagerSignals.DoGameLoaded(saveDataKey);
            }
        }

        public void Delete(string saveDataKey)
        {
            PlayerPrefs.DeleteKey(saveDataKey);
            PlayerPrefs.Save();
        }

        public bool SaveDataExists(string saveDataKey)
        {
            return PlayerPrefs.HasKey(saveDataKey);
        }

        public virtual void AddSavePoint(string savePointKey, string savePointDescription)
        {
            saveHistory.AddSavePoint(savePointKey, savePointDescription);

            SaveManagerSignals.DoSavePointAdded(savePointKey, savePointDescription);
        }

        public virtual void Rewind()
        {
            if (saveHistory.NumSavePoints > 0)
            {
                saveHistory.Rewind();
                saveHistory.LoadLatestSavePoint();
            }
        }

        public virtual void FastForward()
        {
            if (saveHistory.NumRewoundSavePoints > 0)
            {
                saveHistory.FastForward();
                saveHistory.LoadLatestSavePoint();
            }
        }

        public virtual void ClearHistory()
        {
            saveHistory.Clear();
        }

        #endregion
    }
}