// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Variable", 
        "Load Flowchart", 
        "Loads a previously saved Flowchart state. The original scene is loaded and the resume block is executed.")]
    public class LoadFlowchart : Command
    {
        [Tooltip("Key for loading the saves data from PlayerPrefs. Supports variable subsitution {$VarName} and will prepend a profile name set using Set Save Profile command.")]
        [SerializeField] protected StringData saveKey = new StringData("savedata");

        protected SavePointData tempSaveData;

        protected virtual string CreateSaveKey()
        {
            var flowchart = GetFlowchart();
            var saveProfile = SetSaveProfile.SaveProfile;

            if (saveProfile.Length > 0)
            {
                return string.Format(saveProfile + "_" + flowchart.SubstituteVariables(saveKey.Value));
            }
            else
            {
                return string.Format(flowchart.SubstituteVariables(saveKey.Value));
            }
        }

        protected virtual string LoadJSONData(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        protected virtual void LoadSavedState(string jsonData)
        {
            tempSaveData = JsonUtility.FromJson<SavePointData>(jsonData);

            SceneManager.sceneLoaded += OnSceneLoaded;

            // Load scene and wait
            SceneManager.LoadScene(tempSaveData.sceneName);
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == tempSaveData.sceneName)
            {
                SavePointData.ResumeSavedState(tempSaveData);
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region Public members

        public override void OnEnter()
        {
            var key = CreateSaveKey();
            var jsonData = LoadJSONData(key);

            if (jsonData == "")
            {
                // Save data not found, continue executing block
                Continue();
                return;
            }

            LoadSavedState(jsonData);
        }

        public override string GetSummary()
        {
            return saveKey.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}