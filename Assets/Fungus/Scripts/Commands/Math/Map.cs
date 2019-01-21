using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Map a value that exists in 1 range of numbers to another.
    /// </summary>
    [CommandInfo("Math",
                 "Map",
                 "Map a value that exists in 1 range of numbers to another.")]
    [AddComponentMenu("")]
    public class Map : Command
    {
        //[Tooltip("LHS Value ")]
        [SerializeField]
        protected FloatData initialRangeLower = new FloatData(0), initialRangeUpper = new FloatData(1), value;
        
        [SerializeField]
        protected FloatData newRangeLower = new FloatData(0), newRangeUpper = new FloatData(1);
        
        [SerializeField]
        protected FloatData outValue;

        public override void OnEnter()
        {
            var p = value.Value - initialRangeLower.Value;
            p /= initialRangeUpper.Value - initialRangeLower.Value;

            var res = p * (newRangeUpper.Value - newRangeLower.Value);
            res += newRangeLower.Value;

            outValue.Value = res;

            Continue();
        }

        public override string GetSummary()
        {
            return "Map [" + initialRangeLower.Value.ToString() + "-" + initialRangeUpper.Value.ToString() + "] to [" +
                newRangeLower.Value.ToString() + "-" + newRangeUpper.Value.ToString() + "]";
        }

        public override bool HasReference(Variable variable)
        {
            return initialRangeLower.floatRef == variable || initialRangeUpper.floatRef == variable || value.floatRef == variable ||
                   newRangeLower.floatRef == variable || newRangeUpper.floatRef == variable ||
                   outValue.floatRef == variable;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}