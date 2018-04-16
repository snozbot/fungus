// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

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
                          typeof(StringVariable),
                          typeof(GameObjectVariable))]
        [SerializeField] protected Variable variable;

        [Tooltip("Boolean value to compare against")]
        [SerializeField] protected BooleanData booleanData;

        [Tooltip("Integer value to compare against")]
        [SerializeField] protected IntegerData integerData;

        [Tooltip("Float value to compare against")]
        [SerializeField] protected FloatData floatData;

        [Tooltip("String value to compare against")]
        [SerializeField] protected StringDataMulti stringData;

        [Tooltip("GameObject value to compare against")]
        [SerializeField] protected GameObjectData gameObjectData;

        protected override bool EvaluateCondition()
        {
            if (variable == null)
            {
                return false;
            }

            bool condition = false;

            if (variable.GetType() == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = (variable as BooleanVariable);
                condition = booleanVariable.Evaluate(compareOperator, booleanData.Value);
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = (variable as IntegerVariable);
                condition = integerVariable.Evaluate(compareOperator, integerData.Value);
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                FloatVariable floatVariable = (variable as FloatVariable);
                condition = floatVariable.Evaluate(compareOperator, floatData.Value);
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                StringVariable stringVariable = (variable as StringVariable);
                condition = stringVariable.Evaluate(compareOperator, stringData.Value);
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                GameObjectVariable gameObjectVariable = (variable as GameObjectVariable);
                condition = gameObjectVariable.Evaluate(compareOperator, gameObjectData.Value);
            }

            return condition;
        }

        protected override bool HasNeededProperties()
        {
            return (variable != null);
        }

        #region Public members

        public static readonly Dictionary<System.Type, CompareOperator[]> operatorsByVariableType = new Dictionary<System.Type, CompareOperator[]>() {
            { typeof(BooleanVariable), BooleanVariable.compareOperators },
            { typeof(IntegerVariable), IntegerVariable.compareOperators },
            { typeof(FloatVariable), FloatVariable.compareOperators },
            { typeof(StringVariable), StringVariable.compareOperators },
            { typeof(GameObjectVariable), GameObjectVariable.compareOperators }
        };

        /// <summary>
        /// The type of comparison operation to be performed.
        /// </summary>
        public virtual CompareOperator _CompareOperator { get { return compareOperator; } }

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
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                summary += gameObjectData.GetDescription();
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