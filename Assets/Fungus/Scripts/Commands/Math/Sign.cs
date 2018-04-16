using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a Sign
    /// </summary>
    [CommandInfo("Math",
                 "Sign",
                 "Command to execute and store the result of a Sign")]
    [AddComponentMenu("")]
    public class Sign : BaseUnaryMathCommand
    {
        public override void OnEnter()
        {
            outValue.Value = Mathf.Sign(inValue.Value);

            Continue();
        }

        public override string GetSummary()
        {
            return "Sign";
        }
    }
}