using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a Log
    /// </summary>
    [CommandInfo("Math",
                 "Log",
                 "Command to execute and store the result of a Log")]
    [AddComponentMenu("")]
    public class Log : BaseUnaryMathCommand
    {
        public enum Mode
        {
            Base10,
            Natural
        }

        [Tooltip("Which log to use, natural or base 10")]
        [SerializeField]
        protected Mode mode = Mode.Natural;

        public override void OnEnter()
        {
            switch (mode)
            {
                case Mode.Base10:
                    outValue.Value = Mathf.Log10(inValue.Value);
                    break;
                case Mode.Natural:
                    outValue.Value = Mathf.Log(inValue.Value);
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return mode.ToString() + " " + base.GetSummary();
        }
    }
}