// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stores the result of a ToString on given variable in a string.
    /// </summary>
    [CommandInfo("Variable",
                 "To String",
                 "Stores the result of a ToString on given variable in a string.")]
    [AddComponentMenu("")]
    public class ToString : Command
    {
        [Tooltip("Target variable to get String of.")]
        [VariableProperty()]
        [SerializeField] protected Variable variable;

        [Tooltip("Variable to store the result of ToString")]
        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable outValue;

        //[Tooltip("Optional formatting string given to ToString")]
        //[SerializeField] protected StringData format;

        public override void OnEnter()
        {
            if (variable != null && outValue != null)
            {
                outValue.Value = variable.ToString();
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: Variable not selected";
            }

            if (outValue == null)
            {
                return "Error: outValue not set";
            }

            return outValue.Key + " = " + variable.Key + ".ToString";
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable) || outValue == variable;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }
    }
}