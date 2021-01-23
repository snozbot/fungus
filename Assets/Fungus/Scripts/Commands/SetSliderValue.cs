// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Sets or Gets the value property of a slider object.
    /// </summary>
    [CommandInfo("UI",
                 "Set or Get Slider Value",
                 "Sets or Gets the value property of a slider object")]
    public class SetSliderValue : Command
    {
        [Tooltip("Target slider object to set the value on")]
        [SerializeField] protected Slider slider;

        [Tooltip("Float value to set the slider value to.")]
        [SerializeField] protected FloatData value;
        [Tooltip("Time to fade between current volume level and target volume level.")]
        [SerializeField] protected float fadeDuration;

        [Tooltip("Wait until this command has finished before executing the next command.")]
        [SerializeField] protected bool waitUntilFinished = false;

        protected BaseVariableProperty.GetSet getOrSet = BaseVariableProperty.GetSet.Set;

        #region Public members
        protected virtual void SliderValueSet(float target)
        {
            // Fade volume in
            LeanTween.value(slider.gameObject,
                slider.value,
                target,
                fadeDuration
            ).setOnUpdate(
                (float updateVolume) =>
                {
                    slider.value = updateVolume;
                });

            if (waitUntilFinished)
            {
                StartCoroutine(WaitAndContinue());
            }
        }

        protected virtual IEnumerator WaitAndContinue()
        {
            // Wait for slider to reach it's target value
            while (LeanTween.isTweening(slider.gameObject))
            {
                yield return null;
            }

            Continue();
        }
        public override void OnEnter()
        {
            if (slider != null)
            {
                switch (getOrSet)
                {
                    case BaseVariableProperty.GetSet.Get:
                        value.Value = slider.value;
                        break;
                    case BaseVariableProperty.GetSet.Set:
                        SliderValueSet(value);
                        break;
                    default:
                        break;
                }
            }
            if (!waitUntilFinished)
            {
                Continue();
            }
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

            return getOrSet == BaseVariableProperty.GetSet.Set ?
                slider.name + " = " + value.GetDescription() :
                value.GetDescription() + " = " + slider.name;
        }

        public override bool HasReference(Variable variable)
        {
            return value.floatRef == variable || base.HasReference(variable);
        }

        #endregion
    }
}