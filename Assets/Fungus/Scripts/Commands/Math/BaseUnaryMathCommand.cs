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

    }
}