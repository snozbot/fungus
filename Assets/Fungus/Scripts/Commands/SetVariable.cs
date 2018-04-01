// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.
    /// </summary>
    [CommandInfo("Variable", 
                 "Set Variable", 
                 "Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.")]
    [AddComponentMenu("")]
    public class SetVariable : Command 
    {
        [Tooltip("The variable whos value will be set")]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable), 
                          typeof(FloatVariable), 
                          typeof(StringVariable),
                          typeof(GameObjectVariable))]
        [SerializeField] protected Variable variable;

        [Tooltip("The type of math operation to be performed")]
        [SerializeField] protected SetOperator setOperator;

        [Tooltip("Boolean value to set with")]
        [SerializeField] protected BooleanData booleanData;

        [Tooltip("Integer value to set with")]
        [SerializeField] protected IntegerData integerData;

        [Tooltip("Float value to set with")]
        [SerializeField] protected FloatData floatData;

        [Tooltip("String value to set with")]
        [SerializeField] protected StringDataMulti stringData;

        [Tooltip("GameObject value to set with")]
        [SerializeField] protected GameObjectData gameObjectData;

        protected virtual void DoSetOperation()
        {
            if (variable == null)
            {
                return;
            }

            if (variable.GetType() == typeof(BooleanVariable))
            {
                BooleanVariable booleanVariable = (variable as BooleanVariable);
                booleanVariable.Apply(setOperator, booleanData.Value);
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                IntegerVariable integerVariable = (variable as IntegerVariable);
                integerVariable.Apply(setOperator, integerData.Value);
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                FloatVariable floatVariable = (variable as FloatVariable);
                floatVariable.Apply(setOperator, floatData.Value);
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                StringVariable stringVariable = (variable as StringVariable);
                var flowchart = GetFlowchart();
                stringVariable.Apply(setOperator, flowchart.SubstituteVariables(stringData.Value));
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                GameObjectVariable gameObjectVariable = (variable as GameObjectVariable);
                gameObjectVariable.Apply(setOperator, gameObjectData.Value);
            }
        }

        #region Public members

        public static readonly Dictionary<System.Type, SetOperator[]> operatorsByVariableType = new Dictionary<System.Type, SetOperator[]>() {
            { typeof(BooleanVariable), BooleanVariable.setOperators },
            { typeof(IntegerVariable), IntegerVariable.setOperators },
            { typeof(FloatVariable), FloatVariable.setOperators },
            { typeof(StringVariable), StringVariable.setOperators },
            { typeof(GameObjectVariable), GameObjectVariable.setOperators }
        };

        /// <summary>
        /// The type of math operation to be performed.
        /// </summary>
        public virtual SetOperator _SetOperator { get { return setOperator; } }

        public override void OnEnter()
        {
            DoSetOperation();

            Continue();
        }

        public override string GetSummary()
        {
            if (variable == null)
            {
                return "Error: Variable not selected";
            }

            string description = variable.Key;

            switch (setOperator)
            {
            default:
            case SetOperator.Assign:
                description += " = ";
                break;
            case SetOperator.Negate:
                description += " =! ";
                break;
            case SetOperator.Add:
                description += " += ";
                break;
            case SetOperator.Subtract:
                description += " -= ";
                break;
            case SetOperator.Multiply:
                description += " *= ";
                break;
            case SetOperator.Divide:
                description += " /= ";
                break;
            }

            if (variable.GetType() == typeof(BooleanVariable))
            {
                description += booleanData.GetDescription();
            }
            else if (variable.GetType() == typeof(IntegerVariable))
            {
                description += integerData.GetDescription();
            }
            else if (variable.GetType() == typeof(FloatVariable))
            {
                description += floatData.GetDescription();
            }
            else if (variable.GetType() == typeof(StringVariable))
            {
                description += stringData.GetDescription();
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                description += gameObjectData.GetDescription();
            }

            return description;
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
