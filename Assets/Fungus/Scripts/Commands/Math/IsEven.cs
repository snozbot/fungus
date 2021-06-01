using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Is Even",
                 "Returns true if the input number is even.")]
    [AddComponentMenu("")]
    public class IsEven : Condition
    {
        [Tooltip("Variable to check.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable variable;
        float DivideBy = 2;
        float remainder = 0;

        public override string GetSummary()
        {
            if (variable != null) return "Is (" + variable.Key + ") " + variable.GetValue() + " even?";
            else return "Error: [Null Variable]";
        }

        protected override bool EvaluateCondition()
        {
            if (variable is IntegerVariable) return (int)variable.GetValue() % DivideBy == remainder;
            else return (float)variable.GetValue() % DivideBy == remainder;
        }

        public override bool HasReference(Variable Variable)
        {
            return variable == Variable;
        }
    }
}