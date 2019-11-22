// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
    /// <summary>
    /// The Save History is a list of previously created Save Points, sorted chronologically.
    /// </summary>
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

        /// <summary>
        /// Returns the number of Save Points in the Save History.
        /// </summary>
        public int NumSavePoints { get { return savePoints.Count; } }

        /// <summary>
        /// Returns the current number of rewound Save Points in the Save History.
        /// </summary>
        public int NumRewoundSavePoints { get { return rewoundSavePoints.Count; } }

        /// <summary>
        /// Creates a new Save Point using a key and description, and adds it to the Save History.
        /// </summary>
        public void AddSavePoint(string savePointKey, string savePointDescription)
        {
            // Creating a new Save Point invalidates all rewound Save Points, so delete them.
            ClearRewoundSavePoints();

            string sceneName = SceneManager.GetActiveScene().name;
            var savePointData = SavePointData.Encode(savePointKey, savePointDescription, sceneName);
            savePoints.Add(savePointData);
        }

        /// <summary>
        /// Rewinds to the previous Save Point in the Save History.
        /// The latest Save Point is moved to a separate list of rewound save points.
        /// </summary>
        public void Rewind()
        {
            if (savePoints.Count > 0)
            {
                rewoundSavePoints.Add(savePoints[savePoints.Count - 1]);
                savePoints.RemoveAt(savePoints.Count - 1);
            }
        }

        /// <summary>
        /// Fast forwards to the next Save Point in the Save History.
        /// The most recently rewound Save Point is moved back to the main list of save points.
        /// </summary>
        public void FastForward()
        {
            if (rewoundSavePoints.Count > 0)
            {
                savePoints.Add(rewoundSavePoints[rewoundSavePoints.Count - 1]);
                rewoundSavePoints.RemoveAt(rewoundSavePoints.Count - 1);
            }
        }

        /// <summary>
        /// Loads the latest Save Point.
        /// </summary>
        public void LoadLatestSavePoint()
        {
            if (savePoints.Count > 0)
            {
                var savePointData = savePoints[savePoints.Count - 1];
                SavePointData.Decode(savePointData);
            }
        }

        /// <summary>
        /// Clears all Save Points.
        /// </summary>
        public void Clear()
        {
            savePoints.Clear();
            rewoundSavePoints.Clear();
        }

        /// <summary>
        /// Clears rewound Save Points only. The main Save Point list is not changed.
        /// </summary>
        public void ClearRewoundSavePoints()
        {
            rewoundSavePoints.Clear();
        }

        public virtual string GetDebugInfo()
        {
            string debugInfo = "Save points:\n";

            foreach (var savePoint in savePoints)
            {
                debugInfo += savePoint.Substring(0, savePoint.IndexOf(',')).Replace("\n", "").Replace("{", "").Replace("}","") + "\n";
            }

            debugInfo += "Rewound points:\n";

            foreach (var savePoint in rewoundSavePoints)
            {
                debugInfo += savePoint.Substring(0, savePoint.IndexOf(',')).Replace("\n", "").Replace("{", "").Replace("}","") + "\n";
            }

            return debugInfo;
        }

        #endregion
    }
}

#endif