using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveManager : MonoBehaviour 
    {
        const string ActiveSlotKey = "active_slot";

        const string SlotKeyFormat = "slot{0}";

        // Make serialize data extensible (subclassing?)
        // Save key, use save profile and variable substitution
        // Store scene name, flowchart name and block name to execute after load
        // Show link to Block to be executed
        // Handle New Save Slot case: Scene to load?
        // Save command stores data in SaveManager for writing later
        // If SaveImmediately is selected then save it straight away (SaveGame command)
        // If not selected, then Save when a Save button is pressed
        // Select / Load button - set active slot (in playerprefs) and load the state
        // Get list of saved games
        // Delete Save Game
        // Rename SaveFlowchart to SaveGame

        protected string saveBuffer = "";

        protected static SaveManager instance;

        protected SavePointData tempSaveData;

        protected virtual void Awake()
        {
            instance = this;
        }

        protected virtual string FormatSaveKey(int slot)
        {
            return string.Format(SlotKeyFormat, slot);
        }

        protected virtual bool LoadNewGame(string key, string startScene)
        {
            if (PlayerPrefs.HasKey(key) &&
                PlayerPrefs.GetString(key) != "")
            {
                return false;
            }

            // Create a new save entry
            PlayerPrefs.SetString(key, "");

            // Load the start scene
            SceneManager.LoadScene(startScene);

            return true;
        }

        protected virtual string CreateSaveData(Flowchart flowchart, string saveKey)
        {
            var saveData = new SavePointData();

            // Store the scene, flowchart and block to execute on resume
            saveData.SceneName = SceneManager.GetActiveScene().name;
            saveData.SaveKey = saveKey;

            var flowchartData = FlowchartData.Encode(flowchart);
            saveData.FlowchartData.Add(flowchartData);

            return JsonUtility.ToJson(saveData, true);
        }

        protected virtual void RestoreSavedGame(SavePointData saveData)
        {
            var flowchartData = saveData.FlowchartData[0];

            FlowchartData.Decode(flowchartData);

            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(saveData.SaveKey);
            } 

            // Execute any block with a Label matching the save key
            var labels = Object.FindObjectsOfType<Label>();
            for (int i = 0; i < labels.Length; i++)
            {
                var label = labels[i];
                if (string.Compare(label.Key, saveData.SaveKey, true) == 0)
                {
                    int index = label.CommandIndex;
                    var block = label.ParentBlock;
                    var fc = label.GetFlowchart();
                    fc.ExecuteBlock(block, index + 1);
                }
            }
        }

        protected virtual void StoreJSONData(string key, string jsonData)
        {
            if (key.Length > 0)
            {
                PlayerPrefs.SetString(key, jsonData);
            }
        }

        protected virtual string LoadJSONData(string key)
        {
            if (key.Length > 0)
            {
                return PlayerPrefs.GetString(key);
            }

            return "";
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == tempSaveData.SceneName)
            {
                RestoreSavedGame(tempSaveData);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region Public members

        public static SaveManager Instance { get { return instance; } }

        public virtual int ActiveSlot
        {
            get
            {
                return PlayerPrefs.GetInt(ActiveSlotKey);
            }
            set
            {
                PlayerPrefs.SetInt(ActiveSlotKey, value);
            }
        }

        public virtual void Save()
        {
            if (saveBuffer == "")
            {
                // Nothing to save
                return;
            }

            var key = FormatSaveKey(ActiveSlot);

            PlayerPrefs.SetString(key, saveBuffer);

            saveBuffer = "";
        }

        public virtual void Load(int slot, string startScene = "")
        {
            ActiveSlot = slot;

            var key = FormatSaveKey(slot);

            if (LoadNewGame(key, startScene))
            {
                return;
            }

            var jsonData = LoadJSONData(key);
            tempSaveData = JsonUtility.FromJson<SavePointData>(jsonData);

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(tempSaveData.SceneName);
        }

        public virtual void Delete(int slot)
        {
            var key = FormatSaveKey(slot);
            PlayerPrefs.DeleteKey(key);
        }

        public virtual void PopulateSaveBuffer(Flowchart flowchart, string saveKey)
        {
            saveBuffer = CreateSaveData(flowchart, saveKey);
        }

        #endregion
    }
}