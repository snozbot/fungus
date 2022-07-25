// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Resets the state of all commands and variables in the Flowchart.
    /// </summary>
    [CommandInfo("Variable", 
                 "Reset", 
                 "Resets the state of all commands and variables in the Flowchart.")]
    [AddComponentMenu("")]
    public class Reset : Command
    {   
        [Tooltip("Reset state of all commands in the script")]
        [SerializeField] protected bool resetCommands = true;

        [Tooltip("Reset variables back to their default values")]
        [SerializeField] protected bool resetVariables = true;

        #region Public members

        public override void OnEnter()
        {
            GetFlowchart().Reset(resetCommands, resetVariables);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}