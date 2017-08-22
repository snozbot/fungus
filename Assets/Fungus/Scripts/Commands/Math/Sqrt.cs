using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a Sqrt
    /// </summary>
    [CommandInfo("Math",
                 "Sqrt",
                 "Command to execute and store the result of a Sqrt")]
    [AddComponentMenu("")]
    public class Sqrt : BaseUnaryMathCommand
    {
        public override void OnEnter()
        {
            outValue.Value = Mathf.Sqrt(inValue.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return "Sqrt";
        }
    }
}