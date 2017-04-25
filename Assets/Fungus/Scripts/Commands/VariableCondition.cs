// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    public abstract class VariableCondition : Condition
    {
        [Tooltip("The type of comparison to be performed")]
        [SerializeField] protected CompareOperator compareOperator;

        [Tooltip("Variable to use in expression")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable))]
        [SerializeField] protected Variable variable;

        [Tooltip("Boolean value to compare against")]
        [SerializeField] protected BooleanData booleanData;

        [Tooltip("Integer value to compare against")]
        [SerializeField] protected IntegerData integerData;

        [Tooltip("Float value to compare against")]
        [SerializeField] protected FloatData floatData;

        [Tooltip("String value to compare against")]
        [SerializeField] protected StringDataMulti stringData;

        protected override bool EvaluateCondition()
        {
            BooleanVariable booleanVariable = variable as BooleanVariable;
            IntegerVariable integerVariable = variable as IntegerVariable;
            FloatVariable floatVariable = variable as FloatVariable;
            StringVariable stringVariable = variable as StringVariable;
            
            bool condition = false;
            
            if (booleanVariable != null)
            {
                condition = booleanVariable.Evaluate(compareOperator, booleanData.Value);
            }
            else if (integerVariable != null)
            {
                condition = integerVariable.Evaluate(compareOperator, integerData.Value);
            }
            else if (floatVariable != null)
            {
                condition = floatVariable.Evaluate(compareOperator, floatData.Value);
            }
            else if (stringVariable != null)
            {
                condition = stringVariable.Evaluate(compareOperator, stringData.Value);
            }

            return condition;
        }

        protected override bool HasNeededProperties()
        {
            return (variable != null);
        }

        #region Public members

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: No variable selected";
            }

            string summary = variable.Key + " ";
            summary += Condition.GetOperatorDescription(compareOperator) + " ";

            if (variable.GetType() == typeof(BooleanVariable))
            {
                summary += booleanData.GetDescription();
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                summary += integerData.GetDescription();
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                summary += floatData.GetDescription();
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                summary += stringData.GetDescription();
            }

            return summary;
        }

        public override bool HasReference(Variable variable)
        {
            return (variable == this.variable);
        }

        public override Color GetButtonColor()
        {
            return new Color32(253, 253, 150, 255);
        }

        #endregion
    }
}