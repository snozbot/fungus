// This code is part of the Fungus library (https://github.com/snozbot/fungus)
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
        public override bool IsArithmeticSupported(SetOperator setOperator)
        {
            return true;
        }

        public override bool IsComparisonSupported()
        {
            return true;
        }

        public override void Apply(SetOperator setOperator, float value)
        {
            switch (setOperator)
            {
            case SetOperator.Negate:
                Value = Value * -1;
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
                base.Apply(setOperator, value);
                break;
            }
        }

        public override bool Evaluate(CompareOperator compareOperator, float value)
        {
            float lhs = Value;
            float rhs = value;

            bool condition = false;

            switch (compareOperator)
            {
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
                condition = base.Evaluate(compareOperator, value);
                break;
            }

            return condition;
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