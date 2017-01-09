using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            if (scene.name != tempSavePointData.SceneName)
            {
                return;
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;

            var savePointData = tempSavePointData;

            for (int i = 0; i < savePointData.FlowchartDatas.Count; i++)
            {
                var flowchartData = savePointData.FlowchartDatas[i];
                FlowchartData.Decode(flowchartData);
            }

            ExecuteBlocks(savePointData.savePointKey);
        }

        protected static void ExecuteBlocks(string savePointKey)
        {
            SavePointLoaded.NotifyEventHandlers(savePointKey);

            // Execute any block containing a SavePoint command matching the save key, with Resume From Here enabled
            var savePoints = Object.FindObjectsOfType<SavePoint>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                var savePoint = savePoints[i];
                if (savePoint.ResumeFromHere &&
                    string.Compare(savePoint.SavePointKey, savePointKey, true) == 0)
                {
                    int index = savePoint.CommandIndex;
                    var block = savePoint.ParentBlock;
                    var flowchart = savePoint.GetFlowchart();
                    flowchart.ExecuteBlock(block, index + 1);

                    // Assume there's only one SavePoint using this key
                    break;
                }
            }
                
            // Execute any block containing a Label matching the save key
            var labels = Object.FindObjectsOfType<Label>();
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (string.Compare(label.Key, savePointKey, true) == 0)
                {
                    int index = label.CommandIndex;
                    var block = label.ParentBlock;
                    var flowchart = label.GetFlowchart();
                    flowchart.ExecuteBlock(block, index + 1);
                }
            }
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
                
            var saveGameObjects = GameObject.FindObjectOfType<SavedObjects>();
            if (saveGameObjects == null)
            {
                Debug.LogWarning("Failed to find a SaveGameObjects in current scene");
            }
            else
            {
                saveGameObjects.Encode(savePointData);
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