// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a Round
    /// </summary>
    [CommandInfo("Math",
                 "Round",
                 "Command to execute and store the result of a Round")]
    [AddComponentMenu("")]
    public class Round : BaseUnaryMathCommand
    {
        public enum Mode
        {
            Round,
            Floor,
            Ceil
        }

        [Tooltip("Mode; Round (closest), floor(smaller) or ceil(bigger).")]
        [SerializeField]
        protected Mode function = Mode.Round;

        public override void OnEnter()
        {
            switch (function)
            {
                case Mode.Round:
                    outValue.Value = Mathf.Round(inValue.Value);
                    break;
                case Mode.Floor:
                    outValue.Value = Mathf.Floor(inValue.Value);
                    break;
                case Mode.Ceil:
                    outValue.Value = Mathf.Ceil(inValue.Value);
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