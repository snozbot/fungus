// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Deletes a saved value from permanent storage.
    /// </summary>
    [CommandInfo("Variable", 
                 "Delete Save Key", 
                 "Deletes a saved value from permanent storage.")]
    [AddComponentMenu("")]
    public class DeleteSaveKey : Command
    {
        [Tooltip("Name of the saved value. Supports variable substition e.g. \"player_{$PlayerNumber}")]
        [SerializeField] protected string key = "";

        #region Public members

        public override void OnEnter()
        {
            if (key == "")
            {
                Continue();
                return;
            }
            
            var flowchart = GetFlowchart();
            
            // Prepend the current save profile (if any)
            string prefsKey = SetSaveProfile.SaveProfile + "_" + flowchart.SubstituteVariables(key);
            
            PlayerPrefs.DeleteKey(prefsKey);

            Continue();
        }
        
        public override string GetSummary()
        {
            if (key.Length == 0)
            {
                return "Error: No stored value key selected";
            }

            return key;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }    
}