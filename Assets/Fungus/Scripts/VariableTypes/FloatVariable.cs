// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Float variable type.
    /// </summary>
    [VariableInfo("", "Float")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class FloatVariable : VariableBase<float>
    {
        public static readonly CompareOperator[] compareOperators = {
            CompareOperator.Equals,
            CompareOperator.NotEquals,
            CompareOperator.LessThan,
            CompareOperator.GreaterThan,
            CompareOperator.LessThanOrEquals,
            CompareOperator.GreaterThanOrEquals
        };
        public static readonly SetOperator[] setOperators = {
            SetOperator.Assign,
            SetOperator.Add,
            SetOperator.Subtract,
            SetOperator.Multiply,
            SetOperator.Divide
        };

        public virtual bool Evaluate(CompareOperator compareOperator, float floatValue)
        {
            float lhs = Value;
            float rhs = floatValue;
            
            bool condition = false;
            
            switch (compareOperator)
            {
                case CompareOperator.Equals:
                    condition = lhs == rhs;
                    break;
                case CompareOperator.NotEquals:
                    condition = lhs != rhs;
                    break;
                case CompareOperator.LessThan:
                    condition = lhs < rhs;
                    break;
                case CompareOperator.GreaterThan:
                    condition = lhs > rhs;
                    break;
                case CompareOperator.LessThanOrEquals:
                    condition = lhs <= rhs;
                    break;
                case CompareOperator.GreaterThanOrEquals:
                    condition = lhs >= rhs;
                    break;
                default:
                    Debug.LogError("The " + compareOperator.ToString() + " comparison operator is not valid.");
                    break;
            }
            
            return condition;
        }

        public override void Apply(SetOperator setOperator, float value)
        {
            switch (setOperator)
            {
                case SetOperator.Assign:
                    Value = value;
                    break;
                case SetOperator.Add:
                    Value += value;
                    break;
                case SetOperator.Subtract:
                    Value -= value;
                    break;
                case SetOperator.Multiply:
                    Value *= value;
                    break;
                case SetOperator.Divide:
                    Value /= value;
                    break;
                default:
                    Debug.LogError("The " + setOperator.ToString() + " set operator is not valid.");
                    break;
            }
        }
    }

    /// <summary>
    /// Container for an float variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct FloatData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(FloatVariable))]
        public FloatVariable floatRef;

        [SerializeField]
        public float floatVal;

        public FloatData(float v)
        {
            floatVal = v;
            floatRef = null;
        }

        public static implicit operator float(FloatData floatData)
        {
            return floatData.Value;
        }

        public float Value
        {
            get { return (floatRef == null) ? floatVal : floatRef.Value; }
            set { if (floatRef == null) { floatVal = value; } else { floatRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (floatRef == null)
            {
                return floatVal.ToString();
            }
            else
            {
                return floatRef.Key;
            }
        }
    }
}