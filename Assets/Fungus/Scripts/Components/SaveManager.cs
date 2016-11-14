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

        protected virtual string CreateSaveData(Flowchart flowchart, string resumeBlockName)
        {
            var saveData = new SavePointData();

            // Store the scene, flowchart and block to execute on resume
            saveData.sceneName = SceneManager.GetActiveScene().name;
            saveData.flowchartName = flowchart.name;
            saveData.resumeBlockName = resumeBlockName;

            for (int i = 0; i < flowchart.Variables.Count; i++) 
            {
                var v = flowchart.Variables[i];

                // Save string
                var stringVariable = v as StringVariable;
                if (stringVariable != null)
                {
                    var d = new StringVar();
                    d.key = stringVariable.Key;
                    d.value = stringVariable.Value;
                    saveData.stringVars.Add(d);
                }

                // Save int
                var intVariable = v as IntegerVariable;
                if (intVariable != null)
                {
                    var d = new IntVar();
                    d.key = intVariable.Key;
                    d.value = intVariable.Value;
                    saveData.intVars.Add(d);
                }

                // Save float
                var floatVariable = v as FloatVariable;
                if (floatVariable != null)
                {
                    var d = new FloatVar();
                    d.key = floatVariable.Key;
                    d.value = floatVariable.Value;
                    saveData.floatVars.Add(d);
                }

                // Save bool
                var boolVariable = v as BooleanVariable;
                if (boolVariable != null)
                {
                    var d = new BoolVar();
                    d.key = boolVariable.Key;
                    d.value = boolVariable.Value;
                    saveData.boolVars.Add(d);
                }
            }

            return JsonUtility.ToJson(saveData, true);
        }

        protected virtual void StoreJSONData(string key, string jsonData)
        {
            if (key.Length > 0)
            {
                PlayerPrefs.SetString(key, jsonData);
            }
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

            // Load JSON data for active slot
            // Convert to SavePointData
            // Load scene
            // Populate variables
            // Execute Block and Label
        }

        public virtual void Delete(int slot)
        {
            var key = FormatSaveKey(slot);
            PlayerPrefs.DeleteKey(key);
        }

        public virtual void PopulateSaveBuffer(Flowchart flowchart, string resumeBlockName)
        {
            var block = flowchart.GetBlock("BlockName");

            foreach (var command in block.CommandList)
            {
                if (command is Menu)
                {

                }
            }


            saveBuffer = CreateSaveData(flowchart, resumeBlockName);
        }

        #endregion
    }
}