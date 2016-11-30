using UnityEngine;
using System.Collections.Generic;
using Fungus;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [System.Serializable]
    public class SaveHistory
    {
        /// <summary>
        /// Version number of current save data format.
        /// </summary>
        protected const int SaveDataVersion = 1;

        [SerializeField] protected int version = SaveDataVersion;

        [SerializeField] protected List<string> savePoints = new List<string>();

        #region Public methods

        public int NumSavePoints { get { return savePoints.Count; } }

        public void AddSavePoint(string saveKey, string saveDescription)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            var savePointData = SavePointData.Encode(saveKey, saveDescription, sceneName);
            savePoints.Add(savePointData);
        }

        /// <summary>
        /// Removes the latest save point.
        /// </summary>
        public void RemoveSavePoint()
        {
            if (savePoints.Count > 0)
            {
                savePoints.RemoveAt(savePoints.Count - 1);
            }
        }

        public void LoadLatestSavePoint()
        {
            if (savePoints.Count > 0)
            {
                var savePointData = savePoints[savePoints.Count - 1];
                SavePointData.Decode(savePointData);
            }
        }

        public void Clear()
        {
            savePoints.Clear();
        }

        #endregion
    }
}
