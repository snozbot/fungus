using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [System.Serializable]
    public class SavePointData
    {
        [SerializeField] protected string saveKey;
        [SerializeField] protected string description;
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

            ExecuteBlocks(savePointData.saveKey);
        }

        protected static void ExecuteBlocks(string saveKey)
        {
            SavePointLoaded.NotifyEventHandlers(saveKey);

            // Execute any block containing a SavePoint command matching the save key, with Resume From Here enabled
            var savePoints = Object.FindObjectsOfType<SavePoint>();
            for (int i = 0; i < savePoints.Length; i++)
            {
                var savePoint = savePoints[i];
                if (savePoint.ResumeFromHere &&
                    string.Compare(savePoint.SaveKey, saveKey, true) == 0)
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
                if (string.Compare(label.Key, saveKey, true) == 0)
                {
                    int index = label.CommandIndex;
                    var block = label.ParentBlock;
                    var flowchart = label.GetFlowchart();
                    flowchart.ExecuteBlock(block, index + 1);
                }
            }
        }

        protected static SavePointData Create(string _saveKey, string _description, string _sceneName)
        {
            var savePointData = new SavePointData();

            savePointData.saveKey = _saveKey;
            savePointData.description = _description;
            savePointData.sceneName = _sceneName;

            return savePointData;
        }

        #region Public methods

        public string SaveKey { get { return saveKey; } set { saveKey = value; } }
        public string Description { get { return description; } set { description = value; } }
        public string SceneName { get { return sceneName; } set { sceneName = value; } }
        public List<FlowchartData> FlowchartDatas { get { return flowchartDatas; } set { flowchartDatas = value; } }

        public static string EncodeNewGame(string _description, string _sceneName)
        {
            var savePointData = Create("start_game", _description, _sceneName);

            return JsonUtility.ToJson(savePointData, true);
        }

        public static string Encode(string _saveKey, string _description, string _sceneName)
        {
            var savePointData = Create(_saveKey, _description, _sceneName);
                
            var saveGameHelper = GameObject.FindObjectOfType<SaveGameHelper>();
            if (saveGameHelper == null)
            {
                Debug.LogError("Failed to find SaveGameHelper object in scene");
                return null;
            }

            for (int i = 0; i < saveGameHelper.SaveGameObjects.Flowcharts.Count; i++)
            {
                var flowchart = saveGameHelper.SaveGameObjects.Flowcharts[i];
                var flowchartData = FlowchartData.Encode(flowchart);
                savePointData.FlowchartDatas.Add(flowchartData);
            }

            return JsonUtility.ToJson(savePointData, true);
        }

        public static void Decode(string saveDataJSON)
        {
            tempSavePointData = JsonUtility.FromJson<SavePointData>(saveDataJSON);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(tempSavePointData.SceneName);
        }            

        #endregion
    }
}