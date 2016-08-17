/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
    [VariableInfo("", "Integer")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class IntegerVariable : VariableBase<int> 
    {
        public virtual bool Evaluate(CompareOperator compareOperator, int integerValue)
        {
            int lhs = value;
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
    }

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
            get { return (integerRef == null) ? integerVal : integerRef.value; }
            set { if (integerRef == null) { integerVal = value; } else { integerRef.value = value; } }
        }

        public string GetDescription()
        {
            if (integerRef == null)
            {
                return integerVal.ToString();
            }
            else
            {
                return integerRef.key;
            }
        }
    }

}