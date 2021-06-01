using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the modulus is true, execute the following command block.
    /// </summary>
    [CommandInfo("Math",
                 "Modulus",
                 "Performs a modulus operation on Variable and outputs it to Remainder.")]
    [AddComponentMenu("")]
    public class Modulus : Command
    {
        [Tooltip("Variable to check.")]
        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable variable;

        [SerializeField] protected FloatData DivideBy = new FloatData(2);

        [VariableProperty(typeof(IntegerVariable),
                          typeof(FloatVariable))]
        [SerializeField] protected Variable remainderOutput;

        public override void OnEnter()
        {
            float value = 0;
            Flowchart flowchart = GetFlowchart();

            if (variable is IntegerVariable) value = (int)variable.GetValue() % DivideBy.Value;
            else value = (float)variable.GetValue() % DivideBy.Value;

            if (remainderOutput is IntegerVariable) flowchart.SetIntegerVariable(remainderOutput.Key, System.Convert.ToInt32(value));
            else flowchart.SetFloatVariable(remainderOutput.Key, value);
        }

        public override string GetSummary()
        {
            if (variable == null) return "Error: [Null Variable] % " + DivideBy;
            else if (remainderOutput == null) return "Error: [Null remainder output]";
            else return "(" + variable.Key + ") " + variable.GetValue() + " % " + DivideBy.Value + " -> " + remainderOutput.Key;
        }

        public override bool HasReference(Variable Variable)
        {
            return variable == Variable || DivideBy.floatRef == Variable || remainderOutput == Variable;
        }
    }
}