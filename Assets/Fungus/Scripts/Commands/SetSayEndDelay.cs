// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Configure the default delay at the end of a Say.
    /// </summary>
    [CommandInfo("Narrative",
                 "Set Say End Delay",
                 "Configure the default delay at the end of a Say.")]
    public class SetSayEndDelay : Command
    {
        [Tooltip("Negative values are treated as 0.")]
        [SerializeField] private FloatData newDelay = new FloatData(0);

        public override void OnEnter()
        {
            base.OnEnter();

            Say.EndDelay = newDelay;
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return newDelay.floatRef == variable || base.HasReference(variable);
        }

        public override void OnValidate()
        {
            base.OnValidate();
            newDelay.Value = Mathf.Max(0, newDelay.Value);
        }

        public override string GetSummary()
        {
            return newDelay.Value.ToString();
        }
    }
}
