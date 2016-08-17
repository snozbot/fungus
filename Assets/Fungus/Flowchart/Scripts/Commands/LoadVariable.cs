/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Variable", 
                 "Load Variable", 
                 "Loads a saved value and stores it in a Boolean, Integer, Float or String variable. If the key is not found then the variable is not modified.")]
    [AddComponentMenu("")]
    public class LoadVariable : Command
    {
        [Tooltip("Name of the saved value. Supports variable substition e.g. \"player_{$PlayerNumber}\"")]
        public string key = "";

        [Tooltip("Variable to store the value in.")]
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
                    booleanVariable.value = (PlayerPrefs.GetInt(prefsKey) == 1);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    integerVariable.value = PlayerPrefs.GetInt(prefsKey);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    floatVariable.value = PlayerPrefs.GetFloat(prefsKey);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    stringVariable.value = PlayerPrefs.GetString(prefsKey);
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

            return "'" + key + "' into " + variable.key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
    
}