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

        [SerializeField] protected List<string> rewoundSavePoints = new List<string>();

        #region Public methods

        public int NumSavePoints { get { return savePoints.Count; } }

        public int NumRewoundSavePoints { get { return rewoundSavePoints.Count; } }

        public void AddSavePoint(string savePointKey, string savePointDescription)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            var savePointData = SavePointData.Encode(savePointKey, savePointDescription, sceneName);
            savePoints.Add(savePointData);
        }

        public void Rewind()
        {
            if (savePoints.Count > 0)
            {
                rewoundSavePoints.Add(savePoints[savePoints.Count - 1]);
                savePoints.RemoveAt(savePoints.Count - 1);
            }
        }

        public void FastForward()
        {
            if (rewoundSavePoints.Count > 0)
            {
                savePoints.Add(rewoundSavePoints[rewoundSavePoints.Count - 1]);
                rewoundSavePoints.RemoveAt(rewoundSavePoints.Count - 1);
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
