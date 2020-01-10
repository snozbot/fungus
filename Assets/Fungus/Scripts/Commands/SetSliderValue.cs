// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Sets the value property of a slider object.
    /// </summary>
    [CommandInfo("UI",
                 "Set Slider Value",
                 "Sets the value property of a slider object")]
    public class SetSliderValue : Command 
    {
        [Tooltip("Target slider object to set the value on")]
        [SerializeField] protected Slider slider;

        [Tooltip("Float value to set the slider value to.")]
        [SerializeField] protected FloatData value;

        #region Public members

        public override void OnEnter() 
        {
            if (slider != null)
            {
                slider.value = value;
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (slider == null)
            {
                return "Error: Slider object not selected";
            }

            return slider.name + " = " + value.GetDescription();
        }

        public override bool HasReference(Variable variable)
        {
            return value.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}