// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace Fungus
{
    /// <summary>
    /// Save an Boolean, Integer, Float or String variable to persistent storage using a string key.
    /// The value can be loaded again later using the Load Variable command. You can also 
    /// use the Set Save Profile command to manage separate save profiles for multiple players.
    /// </summary>
    [CommandInfo("Variable", 
                 "Save Variable", 
                 "Save an Boolean, Integer, Float or String variable to persistent storage using a string key. " +
                 "The value can be loaded again later using the Load Variable command. You can also " +
                 "use the Set Save Profile command to manage separate save profiles for multiple players.")]
    [AddComponentMenu("")]
    public class SaveVariable : Command
    {
        [Tooltip("Name of the saved value. Supports variable substition e.g. \"player_{$PlayerNumber}")]
        [SerializeField] protected string key = "";
        
        [Tooltip("Variable to read the value from. Only Boolean, Integer, Float and String are supported.")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable))]
        [SerializeField] protected Variable variable;

        #region Public members

        public override void OnEnter()
        {
            if (key == "" ||
                variable == null)
            {
                Continue();
                return;
            }
            
            var flowchart = GetFlowchart();
            
            // Prepend the current save profile (if any)
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);
            ArrayList profileList = memoizeProfileList(SetSaveProfile.SaveProfile);            
            System.Type variableType = variable.GetType();

            if (variableType == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = variable as BooleanVariable;
                if (booleanVariable != null)
                {
                    // PlayerPrefs does not have bool accessors, so just use int
                    PlayerPrefs.SetInt(prefsKey, booleanVariable.Value ? 1 : 0);
                }
            }
            else if (variableType == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = variable as IntegerVariable;
                if (integerVariable != null)
                {
                    PlayerPrefs.SetInt(prefsKey, integerVariable.Value);
                }
            }
            else if (variableType == typeof(FloatVariable))
            {
                FloatVariable floatVariable = variable as FloatVariable;
                if (floatVariable != null)
                {
                    PlayerPrefs.SetFloat(prefsKey, floatVariable.Value);
                }
            }
            else if (variableType == typeof(StringVariable))
            {
                StringVariable stringVariable = variable as StringVariable;
                if (stringVariable != null)
                {
                    PlayerPrefs.SetString(prefsKey, stringVariable.Value);
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
            
            return variable.Key + " into '" + key + "'";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }    
    protected ArrayList memoizeProfileList(String name)
    {
        String k= "Fungus.profile_list"
        ArrayList l =null;
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        byte[] bytes;
        if(!PlayerPrefs.HasKey(k))
        {
            l = new ArrayList()
        }
        else
        {
            l=(ArrayList)deserializeProfileList();
        }
        l.Add(name);
        formatter.Serialize(stream,l);
        bytes = memoryStream.ToArray();
        String base64 = Convert.ToBase64String(bytes);
        PlayerPrefs.SetString(prefsKey, stringVariable.Value);
    }
    protected List<String> deserializeProfileList()
    {
        byte[] bytes;
        BinaryFormatter formatter = new BinaryFormatter();
        String s = PlayerPrefs.GetString(k);
        bytes=Convert.FromBase64String(s);
        return formatter.Deserialize(bytes);
    }
}
