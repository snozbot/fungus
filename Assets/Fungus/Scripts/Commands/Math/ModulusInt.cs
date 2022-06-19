using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Modulus - Int",
                 "Performs a modulus operation on Input and outputs it to Remainder.")]
    [AddComponentMenu("")]
    public class ModulusInt : Command
    {
        [Tooltip("Input to calculate.")]
        [SerializeField] protected IntegerData input;

        [SerializeField] protected IntegerData DivideBy = new IntegerData(2);

        [Tooltip("Variable to output remainder to.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable remainderOutput;

        public override void OnEnter()
        {
            Flowchart flowchart = GetFlowchart();

            flowchart.SetIntegerVariable(remainderOutput.Key, input.Value % DivideBy.Value);
        }

        public override string GetSummary()
        {
            string _input = input.integerRef != null ? "(" + input.integerRef.Key + ") " + input.Value : input.Value.ToString();
            string _divideBy = DivideBy.integerRef != null ? "(" + DivideBy.integerRef.Key + ") " + DivideBy.Value : DivideBy.Value.ToString();

            if (remainderOutput == null) return "Error: [Null Output Variable: remainderOutput]";

            return _input + " % " + _divideBy + " -> " + remainderOutput.Key;
        }

        public override bool HasReference(Variable Variable)
        {
            return input.integerRef == Variable || DivideBy.integerRef == Variable || remainderOutput == Variable;
        }
    }
}