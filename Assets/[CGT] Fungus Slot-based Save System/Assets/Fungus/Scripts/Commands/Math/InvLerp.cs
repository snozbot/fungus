using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Calculates the inverse lerp, the percentage a value is between two others.
    /// </summary>
    [CommandInfo("Math",
                 "InvLerp",
                 "Calculates the inverse lerp, the percentage a value is between two others.")]
    [AddComponentMenu("")]
    public class InvLerp : Command
    {
        [Tooltip("Clamp percentage to 0-1?")]
        [SerializeField]
        protected bool clampResult = true;

        //[Tooltip("LHS Value ")]
        [SerializeField]
        protected FloatData a, b, value;

        //[Tooltip("Where the result of the function is stored.")]
        [SerializeField]
        protected FloatData outValue;

        public override void OnEnter()
        {
            if (clampResult)
                outValue.Value = Mathf.InverseLerp(a.Value, b.Value, value.Value);
            else
                outValue.Value = (value.Value - a.Value) / (b.Value - a.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return "InvLerp [" + a.Value.ToString() + "-" + b.Value.ToString() + "]";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}