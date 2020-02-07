// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a float to int conversion
    /// </summary>
    [CommandInfo("Math",
                 "ToInt",
                 "Command to execute and store the result of a float to int conversion")]
    [AddComponentMenu("")]
    public class ToInt : Command
    {
        public enum Mode
        {
            RoundToInt,
            FloorToInt,
            CeilToInt,
        }


        [Tooltip("To integer mode; round, floor or ceil.")]
        [SerializeField]
        protected Mode function = Mode.RoundToInt;

        [Tooltip("Value to be passed in to the function.")]
        [SerializeField]
        protected FloatData inValue;

        [Tooltip("Where the result of the function is stored.")]
        [SerializeField]
        protected IntegerData outValue;

        public override void OnEnter()
        {
            switch (function)
            {
            case Mode.RoundToInt:
                outValue.Value = Mathf.RoundToInt(inValue.Value);
                break;
            case Mode.FloorToInt:
                outValue.Value = Mathf.FloorToInt(inValue.Value);
                break;
            case Mode.CeilToInt:
                outValue.Value = Mathf.CeilToInt(inValue.Value);
                break;
            default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return function.ToString() + 
                   " in: " + (inValue.floatRef != null ? inValue.floatRef.Key : inValue.Value.ToString()) + 
                   ", out: " + (outValue.integerRef != null ? outValue.integerRef.Key : outValue.Value.ToString()); ;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == inValue.floatRef || variable == outValue.integerRef;
        }
    }
}