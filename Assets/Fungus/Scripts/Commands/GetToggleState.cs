// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Gets the state of a toggle UI object and stores it in a boolean variable.
    /// </summary>
    [CommandInfo("UI",
                 "Get Toggle State",
                 "Gets the state of a toggle UI object and stores it in a boolean variable.")]
    public class GetToggleState : Command 
    {
        [Tooltip("Target toggle object to get the value from")]
        [SerializeField] protected Toggle toggle;

        [Tooltip("Boolean variable to store the state of the toggle value in.")]
        [VariableProperty(typeof(BooleanVariable))]
        [SerializeField] protected BooleanVariable toggleState;

        #region Public members

        public override void OnEnter() 
        {
            if (toggle != null &&
                toggleState != null)
            {
                toggleState.Value = toggle.isOn;
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (toggle == null)
            {
                return "Error: Toggle object not selected";
            }

            if (toggleState == null)
            {
                return "Error: Toggle state variable not selected";
            }

            return toggle.name;
        }

        #endregion
    }
}