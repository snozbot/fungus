using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("", "Float")]
	[AddComponentMenu("")]
	public class FloatVariable : VariableBase<float>
	{
		public virtual bool Evaluate(CompareOperator compareOperator, float floatValue)
		{
			float lhs = value;
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
			}
			
			return condition;
		}
	}

	[System.Serializable]
	public struct FloatData
	{
		[SerializeField]
		public FloatVariable floatRef;

		[SerializeField]
		public float floatVal;

		public float Value
		{
			get { return (floatRef == null) ? floatVal : floatRef.value; }
			set { if (floatRef == null) { floatVal = value; } else { floatRef.value = value; } }
		}

		public string GetDescription()
		{
			if (floatRef == null)
			{
				return floatVal.ToString();
			}
			else
			{
				return floatRef.key;
			}
		}
	}

}