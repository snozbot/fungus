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

            NotifyEventHandlers(savePointData);

            ExecuteBlocks(savePointData);
        }

        protected static void NotifyEventHandlers(SavePointData savePointData)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(savePointData.SaveKey);
            }
        }

        protected static void ExecuteBlocks(SavePointData savePointData)
        {
            // Execute any block containing a Label matching the save key
            var labels = Object.FindObjectsOfType<Label>();
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (string.Compare(label.Key, savePointData.SaveKey, true) == 0)
                {
                    int index = label.CommandIndex;
                    var block = label.ParentBlock;
                    var flowchart = label.GetFlowchart();
                    flowchart.ExecuteBlock(block, index + 1);
                }
            }
        }

        #region Public methods

        public string SaveKey { get { return saveKey; } set { saveKey = value; } }
        public string Description { get { return description; } set { description = value; } }
        public string SceneName { get { return sceneName; } set { sceneName = value; } }
        public List<FlowchartData> FlowchartDatas { get { return flowchartDatas; } set { flowchartDatas = value; } }

        public static string Encode(string _saveKey, string _description, string _sceneName)
        {
            var savePointData = new SavePointData();

            savePointData.saveKey = _saveKey;
            savePointData.description = _description;
            savePointData.sceneName = _sceneName;
                
            var saveHelper = GameObject.FindObjectOfType<SaveHelper>();
            if (saveHelper == null)
            {
                Debug.LogError("Failed to find SaveHelper object in scene");
                return null;
            }

            for (int i = 0; i < saveHelper.Flowcharts.Count; i++)
            {
                var flowchart = saveHelper.Flowcharts[i];
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