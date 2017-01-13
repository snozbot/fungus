using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Serializable container for a Save Point.
    /// </summary>
    [System.Serializable]
    public class SavePointData
    {
        [SerializeField] protected string savePointKey;
        [SerializeField] protected string savePointDescription;
        [SerializeField] protected string sceneName;
        [SerializeField] protected List<FlowchartData> flowchartDatas = new List<FlowchartData>();

        protected static SavePointData tempSavePointData;

        protected static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Additive scene loads and non-matching scene loads could happen if someone is using
            // Fungus as part of a bigger game, so just ignore these events and hope they know what they're doing.
            if (mode == LoadSceneMode.Additive ||
                scene.name != tempSavePointData.SceneName)
            {
                return;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;

            var saveDatas = GameObject.FindObjectsOfType<SaveData>();
            foreach (var saveData in saveDatas)
            {
                saveData.Decode(tempSavePointData);
            }

            SaveManagerSignals.DoSavePointLoaded(tempSavePointData.savePointKey);
        }
            
        protected static SavePointData Create(string _savePointKey, string _savePointDescription, string _sceneName)
        {
            var savePointData = new SavePointData();

            savePointData.savePointKey = _savePointKey;
            savePointData.savePointDescription = _savePointDescription;
            savePointData.sceneName = _sceneName;

            return savePointData;
        }

        #region Public methods

        /// <summary>
        /// Gets or sets the unique key for the Save Point.
        /// </summary>
        public string SavePointKey { get { return savePointKey; } set { savePointKey = value; } }

        /// <summary>
        /// Gets or sets the description for the Save Point.
        /// </summary>
        public string SavePointDescription { get { return savePointDescription; } set { savePointDescription = value; } }

        /// <summary>
        /// Gets or sets the scene name associated with the Save Point.
        /// </summary>
        public string SceneName { get { return sceneName; } set { sceneName = value; } }

        /// <summary>
        /// Gets or sets the encoded Flowchart data for the Save Point.
        /// </summary>
        public List<FlowchartData> FlowchartDatas { get { return flowchartDatas; } set { flowchartDatas = value; } }

        /// <summary>
        /// Encodes a new Save Point to data and converts it to JSON text format.
        /// </summary>
        public static string Encode(string _savePointKey, string _savePointDescription, string _sceneName)
        {
            var savePointData = Create(_savePointKey, _savePointDescription, _sceneName);
                
            var saveDatas = GameObject.FindObjectsOfType<SaveData>();
            foreach (var saveData in saveDatas)
            {
                saveData.Encode(savePointData);
            }

            return JsonUtility.ToJson(savePointData, true);
        }

        /// <summary>
        /// Decodes a Save Point from JSON text format and loads it.
        /// </summary>
        public static void Decode(string saveDataJSON)
        {
            tempSavePointData = JsonUtility.FromJson<SavePointData>(saveDataJSON);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(tempSavePointData.SceneName);
        }     

        #endregion
    }
}