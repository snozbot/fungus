using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Pass a value through an AnimationCurve
    /// </summary>
    [CommandInfo("Math",
                 "Curve",
                 "Pass a value through an AnimationCurve")]
    [AddComponentMenu("")]
    public class Curve : BaseUnaryMathCommand
    {
        [SerializeField]
        protected AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

        public override void OnEnter()
        {
            outValue.Value = curve.Evaluate(inValue.Value);

            Continue();
        }
    }
}