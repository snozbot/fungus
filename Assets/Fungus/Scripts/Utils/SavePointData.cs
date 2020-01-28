// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using System.Collections.Generic;
using UnityEngine;


//TODO perhaps want concept of generic history to store data in for cookies and menu has been visited
// add last written time stamp?
//  doco update

namespace Fungus
{
    /// <summary>
    /// Serializable container for a Save Point's data. 
    /// All data is stored as strings, and the only concrete game class it depends on is the SaveData component.
    /// </summary>
    [System.Serializable]
    public class SavePointData
    {
        /// <summary>
        /// Version number of current save data format.
        /// </summary>
        protected const int SaveDataVersion = 2;

        [SerializeField] protected int version = SaveDataVersion;

        [SerializeField] protected string saveName;

        [SerializeField] protected string progressMarkerName;

        [SerializeField] protected string savePointDescription;

        [SerializeField] protected string sceneName;

        [SerializeField] protected List<SaveDataItem> saveDataItems = new List<SaveDataItem>();

        [SerializeField] protected string lastWrittenDateTimeString;

        protected static SavePointData Create(string _saveName, string _savePointDescription, string _sceneName)
        {
            return new SavePointData
            {
                saveName = _saveName,
                savePointDescription = _savePointDescription,
                progressMarkerName = ProgressMarker.LatestExecuted != null ? ProgressMarker.LatestExecuted.CustomKey : string.Empty,
                //TODO include last item in narrative log?
                sceneName = _sceneName,
                lastWrittenDateTimeString = System.DateTime.Now.ToString("O"),
            };
        }

        /// <summary>
        /// Gets or sets the unique key for the Save Point.
        /// </summary>
        public string SaveName { get { return saveName; } set { saveName = value; } }

        public string ProgressMarkerName { get { return progressMarkerName; } set { progressMarkerName = value; } }

        /// <summary>
        /// Gets or sets the description for the Save Point.
        /// </summary>
        public string SavePointDescription { get { return savePointDescription; } set { savePointDescription = value; } }

        /// <summary>
        /// Gets or sets the scene name associated with the Save Point.
        /// </summary>
        public string SceneName { get { return sceneName; } set { sceneName = value; } }

        /// <summary>
        /// Gets the list of save data items.
        /// </summary>
        /// <value>The save data items.</value>
        public List<SaveDataItem> SaveDataItems { get { return saveDataItems; } }

        public System.DateTime LastWritten { get { return System.DateTime.Parse(lastWrittenDateTimeString); } }

        /// <summary>
        /// Encodes a new Save Point to data and converts it to JSON text format.
        /// </summary>
        public static string EncodeToJson(string _saveName, string _savePointDescription, string _sceneName, out SavePointData savePointData)
        {
            savePointData = Create(_saveName, _savePointDescription, _sceneName);

            // Look for a SaveData component in the scene to populate the save data items.
            var savers = GameObject.FindObjectsOfType<SaveDataSerializer>();
            foreach (var saveData in savers)
            {
                saveData.Encode(savePointData);
            }

            return JsonUtility.ToJson(savePointData, true);
        }

        static public SavePointData DecodeFromJSON(string saveDataJSON)
        {
            var savePointData = JsonUtility.FromJson<SavePointData>(saveDataJSON);

            if (savePointData != null && savePointData.version != SaveDataVersion)
            {
                var success = savePointData.HandleVersionMismatch(savePointData.version, SaveDataVersion);

                if (success)
                {
                    savePointData.version = SaveDataVersion;
                }
                else
                {
                    Debug.LogError(savePointData.saveName + " could not be updated from " +
                        savePointData.version.ToString() + " to " + SaveDataVersion.ToString());
                    return null;
                }
            }

            return savePointData;
        }

        public void RunDeserialize()
        {
            if (!string.IsNullOrEmpty(progressMarkerName))
                ProgressMarker.LatestExecuted = ProgressMarker.FindWithKey(progressMarkerName);

            var sers = GameObject.FindObjectsOfType<SaveDataSerializer>();
            foreach (var serializer in sers)
            {
                serializer.Decode(this);
            }
        }

        protected virtual bool HandleVersionMismatch(int version, int saveDataVersion)
        {
            return true;
        }
    }
}

#endif