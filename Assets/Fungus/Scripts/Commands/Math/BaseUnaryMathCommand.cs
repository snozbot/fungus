// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base class for all simple Unary
    /// </summary>
    [AddComponentMenu("")]
    public abstract class BaseUnaryMathCommand : Command
    {
        [Tooltip("Value to be passed in to the function.")]
        [SerializeField]
        protected FloatData inValue;

        [Tooltip("Where the result of the function is stored.")]
        [SerializeField]
        protected FloatData outValue;
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            return "in: " + (inValue.floatRef != null ? inValue.floatRef.Key : inValue.Value.ToString()) + 
                   ", out: " + (outValue.floatRef != null ? outValue.floatRef.Key : outValue.Value.ToString());
        }

        public override bool HasReference(Variable variable)
        {
            return variable == inValue.floatRef || variable == outValue.floatRef;
        }
    }
}