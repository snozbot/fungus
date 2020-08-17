// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Serializable container for a Save Point's data.
    /// All data is stored as strings, in SaveDataItems. SaveDataSerializers being responsible for encoding and decoding.
    /// </summary>
    [System.Serializable]
    public class SavePointData
    {
        /// <summary>
        /// Version number of current save data format.
        /// </summary>
        protected const int SaveDataVersion = FungusConstants.CurrentSaveVersion;

        [SerializeField] protected int version = SaveDataVersion;

        [SerializeField] protected string saveName;

        [SerializeField] protected string progressMarkerName;

        [SerializeField] protected string savePointDescription;

        [SerializeField] protected string sceneName;

        [SerializeField] protected List<SaveDataItem> saveDataItems = new List<SaveDataItem>();

        [SerializeField] protected string lastWrittenDateTimeString;

        protected static SavePointData Create(string _saveName, string _savePointDescription)
        {
            return new SavePointData
            {
                saveName = _saveName,
                savePointDescription = _savePointDescription,
                progressMarkerName = ProgressMarker.LatestExecuted != null ? ProgressMarker.LatestExecuted.CustomKey : string.Empty,
                //TODO include last item in narrative log?
                sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                lastWrittenDateTimeString = System.DateTime.Now.ToString("O"),
            };
        }

        /// <summary>
        /// Gets or sets the unique key for the Save Point.
        /// </summary>
        public string SaveName { get { return saveName; } set { saveName = value; } }

        /// <summary>
        /// Gets or sets the name of the most recent ProgressMarker reached.
        /// </summary>
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

        /// <summary>
        /// DateTime this save reports it was written at.
        /// </summary>
        public System.DateTime LastWritten { get { return System.DateTime.Parse(lastWrittenDateTimeString); } }

        /// <summary>
        /// Encodes a new Save Point to data and converts it to JSON text format.
        ///
        /// Fetches all SaveDataSerializers and asks them to encode themselves into the newly created SavePointData
        /// </summary>
        public static string EncodeToJson(string _saveName, string _savePointDescription, out SavePointData savePointData)
        {
            savePointData = Create(_saveName, _savePointDescription);

            // Look for a SaveData component in the scene to populate the save data items.
            var orderedSerializers = SaveDataSerializer.ActiveSerializers;
            foreach (var saveData in orderedSerializers)
            {
                saveData.Encode(savePointData);
            }

            return JsonUtility.ToJson(savePointData, true);
        }

        /// <summary>
        /// Decodes a JSON text formatted Save back into a new Save Point object, for others to use.
        ///
        /// If version mismatch is found, requests HandleVersionMismatch solve it, if that method
        /// returns false, decode is stopped and null returned.
        /// </summary>
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

        /// <summary>
        /// Passes the current state of this SavePointData back through all the active SaveDataSerializers.
        /// First through all Decodes, then through all PostDecodes.
        /// </summary>
        public virtual void RunDeserialize()
        {
            if (!string.IsNullOrEmpty(progressMarkerName))
                ProgressMarker.LatestExecuted = ProgressMarker.FindWithKey(progressMarkerName);

            var orderedSerializers = SaveDataSerializer.ActiveSerializers;

            foreach (var item in orderedSerializers)
            {
                item.PreDecode();
            }

            foreach (var serializer in orderedSerializers)
            {
                serializer.Decode(this);
            }

            foreach (var item in orderedSerializers)
            {
                item.PostDecode();
            }
        }

        /// <summary>
        /// Provided as entrypoint for attempts to update from previous versions of the save format.
        /// Returning false will prevent the save from being returned during a DecodeFromJSON.
        ///
        /// No default provided, you may simply wish to notify user or log an error.
        /// </summary>
        /// <param name="version"></param>
        /// <param name="saveDataVersion"></param>
        /// <returns></returns>
        protected virtual bool HandleVersionMismatch(int version, int saveDataVersion)
        {
            return true;
        }
    }
}