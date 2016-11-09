// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
	[CommandInfo("Variable", 
				 "Save Point", 
				 "Saves current Flowchart state.")]
	public class SavePoint : Command
	{
		[SerializeField] protected string restoreBlock;

		// Move serialization stuff into a seperate class
		// Make serialize data extensible (subclassing?)
		// Save key, use save profile and variable substitution
		// Store scene name, flowchart name and block name to execute after load
		// Show link to Block to be executed

		protected string CreateSaveData()
		{
			var saveData = new SavePointData();

			var flowchart = GetFlowchart();

			// Store the 
			saveData.sceneName = SceneManager.GetActiveScene().name;
			saveData.flowchartName = flowchart.name;
			saveData.blockName = ParentBlock.BlockName;

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

		#region Public members

		public override void OnEnter()
		{
			var saveJSON = CreateSaveData();

			Debug.Log(saveJSON);
		}

		public override string GetSummary()
		{
			return "";
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

		#endregion
	}
}