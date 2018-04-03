// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Integer variable type.
    /// </summary>
    [VariableInfo("", "Integer")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class IntegerVariable : VariableBase<int> 
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

        public virtual bool Evaluate(CompareOperator compareOperator, int integerValue)
        {
            int lhs = Value;
            int rhs = integerValue;

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
            }

            return condition;
        }

        public override void Apply(SetOperator setOperator, int value)
        {
            switch (setOperator)
            {
                default:
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
            }
        }
    }

    /// <summary>
    /// Container for an integer variable reference or constant value.
    /// </summary>
    [System.Serializable]
    public struct IntegerData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(IntegerVariable))]
        public IntegerVariable integerRef;

        [SerializeField]
        public int integerVal;

        public IntegerData(int v)
        {
            integerVal = v;
            integerRef = null;
        }

        public static implicit operator int(IntegerData integerData)
        {
            return integerData.Value;
        }

        public int Value
        {
            get { return (integerRef == null) ? integerVal : integerRef.Value; }
            set { if (integerRef == null) { integerVal = value; } else { integerRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (integerRef == null)
            {
                return integerVal.ToString();
            }
            else
            {
                return integerRef.Key;
            }
        }
    }
}