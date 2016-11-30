using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveManager : MonoBehaviour 
    {
        const string DefaultSaveDataKey = "save_data";

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

        public virtual void Save(string saveDataKey = DefaultSaveDataKey)
        {
            WriteSaveHistory(saveDataKey);
        }
 
        public void Load(string saveDataKey = DefaultSaveDataKey)
        {
            if (ReadSaveHistory(saveDataKey))
            {
                saveHistory.LoadLatestSavePoint();
            }
        }

        public void Delete(string saveDataKey = DefaultSaveDataKey)
        {
            PlayerPrefs.DeleteKey(saveDataKey);
            PlayerPrefs.Save();
        }

        public virtual void AddSavePoint(string saveKey, string saveDescription)
        {
            saveHistory.AddSavePoint(saveKey, saveDescription);
        }

        public virtual void Rewind()
        {
            if (saveHistory.NumSavePoints > 0)
            {
                saveHistory.RemoveSavePoint();
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