using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of basic trigonometry
    /// </summary>
    [CommandInfo("Math",
                 "Trig",
                 "Command to execute and store the result of basic trigonometry")]
    [AddComponentMenu("")]
    public class Trig : BaseUnaryMathCommand
    {
        public enum Function
        {
            Rad2Deg,
            Deg2Rad,
            ACos,
            ASin,
            ATan,
            Cos,
            Sin,
            Tan
        }
        
        [Tooltip("Trigonometric function to run.")]
        [SerializeField]
        protected Function function = Function.Sin;
        
        public override void OnEnter()
        {
            switch (function)
            {
                case Function.Rad2Deg:
                    outValue.Value = inValue.Value * Mathf.Rad2Deg;
                    break;
                case Function.Deg2Rad:
                    outValue.Value = inValue.Value * Mathf.Deg2Rad;
                    break;
                case Function.ACos:
                    outValue.Value = Mathf.Acos(inValue.Value);
                    break;
                case Function.ASin:
                    outValue.Value = Mathf.Asin(inValue.Value);
                    break;
                case Function.ATan:
                    outValue.Value = Mathf.Atan(inValue.Value);
                    break;
                case Function.Cos:
                    outValue.Value = Mathf.Cos(inValue.Value);
                    break;
                case Function.Sin:
                    outValue.Value = Mathf.Sin(inValue.Value);
                    break;
                case Function.Tan:
                    outValue.Value = Mathf.Tan(inValue.Value);
                    break;
                default:
                    break;
            }
            
            Continue();
        }

        public override string GetSummary()
        {
            return function.ToString() + " " + base.GetSummary();
        }
    }
}