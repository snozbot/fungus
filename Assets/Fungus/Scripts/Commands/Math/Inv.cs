// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Multiplicative Inverse of a float (1/f)
    /// </summary>
    [CommandInfo("Math",
                 "Inverse",
                 "Multiplicative Inverse of a float (1/f)")]
    [AddComponentMenu("")]
    public class Inv : BaseUnaryMathCommand
    {
        public override void OnEnter()
        {
            var v = inValue.Value;

            outValue.Value = v != 0 ? (1.0f / inValue.Value) : 0.0f;

            Continue();
        }
    }
}