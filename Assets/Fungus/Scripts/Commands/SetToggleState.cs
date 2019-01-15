// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Sets the state of a toggle UI object.
    /// </summary>
    [CommandInfo("UI",
                 "Set Toggle State",
                 "Sets the state of a toggle UI object")]
    public class SetToggleState : Command 
    {
        [Tooltip("Target toggle object to set the state on")]
        [SerializeField] protected Toggle toggle;

        [Tooltip("Boolean value to set the toggle state to.")]
        [SerializeField] protected BooleanData value;

        #region Public members

        public override void OnEnter() 
        {
            if (toggle != null)
            {
                toggle.isOn = value.Value;
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

            return toggle.name + " = " + value.GetDescription();
        }

        public override bool HasReference(Variable variable)
        {
            return value.booleanRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}