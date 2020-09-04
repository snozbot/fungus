// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Serializable container for a Save Point's data.
    /// All data is stored as strings, in SaveDataItems. SaveDataSerializers being responsible for encoding and decoding.
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public int version = FungusConstants.CurrentSaveVersion;
        public string saveName;
        public string progressMarkerName;
        public string savePointDescription;
        public string sceneName;
        public List<SaveDataItem> saveDataItems = new List<SaveDataItem>();
        public string lastWrittenDateTimeString;

        public SaveData(string _saveName, string _savePointDescription)
        {
            saveName = _saveName;
            savePointDescription = _savePointDescription;
            progressMarkerName = ProgressMarker.LatestExecuted != null ? ProgressMarker.LatestExecuted.CustomKey : string.Empty;
            //TODO include last item in narrative log?
            sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            lastWrittenDateTimeString = System.DateTime.Now.ToString("O");
        }

        /// <summary>
        /// DateTime this save reports it was written at.
        /// </summary>
        public System.DateTime LastWritten { get { return System.DateTime.Parse(lastWrittenDateTimeString); } }
    }
}