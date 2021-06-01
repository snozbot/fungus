using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Modulus",
                 "Performs a modulus operation on Input and outputs it to Remainder.")]
    [AddComponentMenu("")]
    public class Modulus : Command
    {
        [Tooltip("Variable to check.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable input;

        [SerializeField] protected FloatData DivideBy = new FloatData(2);

        [Tooltip("Variable to output remainder to.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable remainderOutput;

        public override void OnEnter()
        {
            float value = 0;
            Flowchart flowchart = GetFlowchart();

            if (input is IntegerVariable) value = (int)input.GetValue() % DivideBy.Value;
            else value = (float)input.GetValue() % DivideBy.Value;

            if (remainderOutput is IntegerVariable) flowchart.SetIntegerVariable(remainderOutput.Key, System.Convert.ToInt32(value));
            else flowchart.SetFloatVariable(remainderOutput.Key, value);
        }

        public override string GetSummary()
        {
            if (input == null) return "Error: [Null Variable]";
            else if (remainderOutput == null) return "Error: [Null remainder output]";
            else return "(" + input.Key + ") " + input.GetValue() + " % " + DivideBy.Value + " -> " + remainderOutput.Key;
        }

        public override bool HasReference(Variable Variable)
        {
            return input == Variable || DivideBy.floatRef == Variable || remainderOutput == Variable;
        }
    }
}