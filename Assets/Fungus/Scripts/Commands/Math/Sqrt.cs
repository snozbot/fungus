// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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
    }
}