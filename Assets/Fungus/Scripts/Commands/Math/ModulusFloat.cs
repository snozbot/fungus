using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Modulus - Float",
                 "Performs a modulus operation on Input and outputs it to Remainder.")]
    [AddComponentMenu("")]
    public class ModulusFloat : Command
    {
        [Tooltip("Input to calculate.")]
        [SerializeField] protected FloatData input;

        [SerializeField] protected FloatData DivideBy = new FloatData(2);

        [Tooltip("Variable to output remainder to.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable remainderOutput;

        public override void OnEnter()
        {
            Flowchart flowchart = GetFlowchart();

            flowchart.SetFloatVariable(remainderOutput.Key, input.Value % DivideBy.Value);
        }

        public override string GetSummary()
        {
            string _input = input.floatRef != null ? "(" + input.floatRef.Key + ") " + input.Value : input.Value.ToString();
            string _divideBy = DivideBy.floatRef != null ? "(" + DivideBy.floatRef.Key + ") " + DivideBy.Value : DivideBy.Value.ToString();

            if (remainderOutput == null) return "Error: [Null Output Variable: remainderOutput]";

            return _input + " % " + _divideBy + " -> " + remainderOutput.Key;
        }

        public override bool HasReference(Variable Variable)
        {
            return input.floatRef == Variable || DivideBy.floatRef == Variable || remainderOutput == Variable;
        }
    }
}