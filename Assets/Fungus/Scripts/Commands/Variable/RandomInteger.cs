// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets an integer variable to a random value in the defined range.
    /// </summary>
    [CommandInfo("Variable", 
                 "Random Integer", 
                 "Sets an integer variable to a random value in the defined range.")]
    [AddComponentMenu("")]
    public class RandomInteger : Command 
    {
        [Tooltip("The variable whos value will be set")]
        [VariableProperty(typeof(IntegerVariable))]
        [SerializeField] protected IntegerVariable variable;

        [Tooltip("Minimum value for random range")]
        [SerializeField] protected IntegerData minValue;

        [Tooltip("Maximum value for random range")]
        [SerializeField] protected IntegerData maxValue;

        #region Public members

        public override void OnEnter()
        {
            if (variable != null)
            {
                variable.Value = Random.Range(minValue.Value, maxValue.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: Variable not selected";
            }

            return variable.Key;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable) || minValue.integerRef == variable || maxValue.integerRef == variable;
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}