using UnityEngine;
using System.Collections;

namespace Fungus
{

	[VariableInfo("", "String")]
	[AddComponentMenu("")]
	public class StringVariable : VariableBase<string>
	{
		public virtual bool Evaluate(CompareOperator compareOperator, string stringValue)
		{
			string lhs = value;
			string rhs = stringValue;

			bool condition = false;

			switch (compareOperator)
			{
			case CompareOperator.Equals:
				condition = lhs == rhs;
				break;
			case CompareOperator.NotEquals:
			default:
				condition = lhs != rhs;
				break;
			}

			return condition;
		}
	}

	[System.Serializable]
	public struct StringData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(StringVariable))]
		public StringVariable stringRef;

		[TextArea(1,10)]
		[SerializeField]
		public string stringVal;

		public StringData(string v)
		{
			stringVal = v;
			stringRef = null;
		}
		
		public static implicit operator string(StringData spriteData)
		{
			return spriteData.Value;
		}

		public string Value
		{
			get { return (stringRef == null) ? stringVal : stringRef.value; }
			set { if (stringRef == null) { stringVal = value; } else { stringRef.value = value; } }
		}

		public string GetDescription()
		{
			if (stringRef == null)
			{
				return stringVal;
			}
			else
			{
				return stringRef.key;
			}
		}
	}

}