// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus 
{
    /// <summary>
    /// The block will execute when the user changes the value of target UI slider.
    /// </summary>
    [EventHandlerInfo("UI",
                      "Slider Changed",
                      "The block will execute when the user changes the value of target UI slider.")]
    [AddComponentMenu("")]
    public class SliderChanged : EventHandler
    {
        [Tooltip("The UI Slider that the user can click on")]
        [SerializeField] protected Slider targetSlider;

        [Tooltip("Optional, this float reference will have the value filled from the onValueChanged from the ")]
        [VariableProperty(typeof(FloatVariable))]
        [SerializeField] protected FloatVariable outFloatVariable;

        public virtual void Start()
        {
            if (targetSlider != null)
            {
                targetSlider.onValueChanged.AddListener(ValueChangedHandler);
            }
        }

        protected virtual void ValueChangedHandler(float arg0)
        {
            if(outFloatVariable != null)
            {
                outFloatVariable.Value = arg0;
            }

            ExecuteBlock();
        }

        public override string GetSummary()
        {
            if (targetSlider != null)
            {
                return targetSlider.name;
            }

            return "Error: no targetSlider set.";
        }
    }
}
