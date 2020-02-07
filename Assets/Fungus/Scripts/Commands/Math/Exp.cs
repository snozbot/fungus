// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Command to execute and store the result of a Exp
    /// </summary>
    [CommandInfo("Math",
                 "Exp",
                 "Command to execute and store the result of a Exp")]
    [AddComponentMenu("")]
    public class Exp : BaseUnaryMathCommand
    {
        public override void OnEnter()
        {
            outValue.Value = Mathf.Exp(inValue.Value);

            Continue();
        }
    }
}