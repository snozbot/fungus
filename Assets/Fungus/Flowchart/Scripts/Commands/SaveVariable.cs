using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Variable", 
	             "Save Variable", 
	             "Save an Boolean, Integer, Float or String variable to persistent storage using a string key. " +
	             "The value can be loaded again later using the Load Variable command. You can also " +
	             "use the Set Save Profile command to manage separate save profiles for multiple players.")]
	[AddComponentMenu("")]
	public class SaveVariable : Command
	{
		[Tooltip("Name of the saved value. Supports variable substition e.g. \"player_{$PlayerNumber}")]
		public string key = "";
		
		[Tooltip("Variable to read the value from. Only Boolean, Integer, Float and String are supported.")]
		[VariableProperty(typeof(BooleanVariable),
		                  typeof(IntegerVariable), 
		                  typeof(FloatVariable), 
		                  typeof(StringVariable))]
		public Variable variable;

		public override void OnEnter()
		{
			if (key == "" ||
			    variable == null)
			{
				Continue();
				return;
			}
			
			Flowchart flowchart = GetFlowchart();
			
			// Prepend the current save profile (if any)
			string prefsKey = SetSaveProfile.saveProfile + "_" + flowchart.SubstituteVariables(key);
			
			System.Type variableType = variable.GetType();

			if (variableType == typeof(BooleanVariable))
			{
				BooleanVariable booleanVariable = variable as BooleanVariable;
				if (booleanVariable != null)
				{
					// PlayerPrefs does not have bool accessors, so just use int
					PlayerPrefs.SetInt(prefsKey, booleanVariable.value ? 1 : 0);
				}
			}
			else if (variableType == typeof(IntegerVariable))
			{
				IntegerVariable integerVariable = variable as IntegerVariable;
				if (integerVariable != null)
				{
					PlayerPrefs.SetInt(prefsKey, integerVariable.value);
				}
			}
			else if (variableType == typeof(FloatVariable))
			{
				FloatVariable floatVariable = variable as FloatVariable;
				if (floatVariable != null)
				{
					PlayerPrefs.SetFloat(prefsKey, floatVariable.value);
				}
			}
			else if (variableType == typeof(StringVariable))
			{
				StringVariable stringVariable = variable as StringVariable;
				if (stringVariable != null)
				{
					PlayerPrefs.SetString(prefsKey, stringVariable.value);
				}
			}
			
			Continue();
		}
		
		public override string GetSummary()
		{
			if (key.Length == 0)
			{
				return "Error: No stored value key selected";
			}
			
			if (variable == null)
			{
				return "Error: No variable selected";
			}
			
			return variable.key + " into '" + key + "'";
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}
	
}