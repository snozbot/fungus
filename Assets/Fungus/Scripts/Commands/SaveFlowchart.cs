// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Variable", 
				 "Save Flowchart", 
				 "Saves the current Flowchart variable state to be loaded again in future.")]
	public class SaveFlowchart : Command
	{
        [Tooltip("Key for storing save data in PlayerPrefs. Supports variable subsitution {$VarName} and will prepend a profile name set using Set Save Profile command.")]
        [SerializeField] protected StringData saveKey = new StringData("savedata");

		[SerializeField] protected Block resumeBlock;

		// Make serialize data extensible (subclassing?)
		// Save key, use save profile and variable substitution
		// Store scene name, flowchart name and block name to execute after load
		// Show link to Block to be executed

		protected virtual string CreateSaveKey()
		{
			var flowchart = GetFlowchart();
			var saveProfile = SetSaveProfile.SaveProfile;

			if (saveProfile.Length > 0)
			{
				return string.Format(saveProfile  + "_" + flowchart.SubstituteVariables(saveKey.Value));
			}
			else
			{
				return string.Format(flowchart.SubstituteVariables(saveKey.Value));
			}
		}

		protected virtual SavePointData CreateSaveData()
		{
            return SavePointData.Create(GetFlowchart(), resumeBlock.BlockName);
		}

		protected virtual void StoreJSONData(string key, string jsonData)
		{
			if (key.Length > 0)
			{
				PlayerPrefs.SetString(key, jsonData);
			}
		}

		#region Public members

		public override void OnEnter()
		{
			var key = CreateSaveKey();

            var saveData = CreateSaveData();
			var saveDataJSON = JsonUtility.ToJson(saveData, true);

			StoreJSONData(key, saveDataJSON);

			Continue();
		}

		public override string GetSummary()
		{
			return saveKey.Value;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (resumeBlock != null)
            {
                connectedBlocks.Add(resumeBlock);
            }
        }

		#endregion
	}
}