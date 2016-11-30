using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveManager : MonoBehaviour 
    {
        const string DefaultSaveDataKey = "save_data";

        protected static SaveHistory saveHistory = new SaveHistory();

        protected virtual void ReadSaveHistory(string saveDataKey)
        {
            var historyData = PlayerPrefs.GetString(saveDataKey);
            if (!string.IsNullOrEmpty(historyData))
            {
                var tempSaveHistory = JsonUtility.FromJson<SaveHistory>(historyData);
                if (tempSaveHistory != null)
                {
                    saveHistory = tempSaveHistory;
                }
            }
        }

        protected virtual void WriteSaveHistory(string saveDataKey)
        {
            var historyData = JsonUtility.ToJson(saveHistory, true);
            if (!string.IsNullOrEmpty(historyData))
            {
                PlayerPrefs.SetString(saveDataKey, historyData);
                PlayerPrefs.Save();
            }
        }

        #region Public members

        public virtual void Save(string saveDataKey = DefaultSaveDataKey)
        {
            WriteSaveHistory(saveDataKey);
        }
 
        public void Load(string saveDataKey = DefaultSaveDataKey)
        {
            ReadSaveHistory(saveDataKey);
            if (saveHistory != null)
            {
                saveHistory.LoadLatestSavePoint();
            }
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

        public virtual void Clear()
        {
            saveHistory.Clear();
        }

        #endregion
    }
}