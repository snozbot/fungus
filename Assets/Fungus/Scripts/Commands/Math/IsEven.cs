using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Is Even",
                 "Returns true if Input is even.")]
    [AddComponentMenu("")]
    public class IsEven : Condition
    {
        [Tooltip("Variable to check.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable input;
        float DivideBy = 2;
        float remainder = 0;

        public override string GetSummary()
        {
            if (input != null) return "Is (" + input.Key + ") " + input.GetValue() + " even?";
            else return "Error: [Null Variable]";
        }

        protected override bool EvaluateCondition()
        {
            if (input is IntegerVariable) return (int)input.GetValue() % DivideBy == remainder;
            else return (float)input.GetValue() % DivideBy == remainder;
        }

        public override bool HasReference(Variable Variable)
        {
            return input == Variable;
        }
    }
}