using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class IntegerVariable : Variable 
	{
		protected int integerValue;

		public int Value
		{
			get { return (scope == VariableScope.Local) ? integerValue : GlobalVariables.GetInteger(key); }
			set { if (scope == VariableScope.Local) { integerValue = value; } else { GlobalVariables.SetInteger(key, value); } }
		}
	}

	[System.Serializable]
	public class IntegerData
	{
		[SerializeField]
		protected IntegerVariable integerReference;

		[SerializeField]
		protected int integerValue;

		public int Value
		{
			get { return (integerReference == null) ? integerValue : integerReference.Value; }
			set { if (integerReference == null) { integerValue = value; } else { integerReference.Value = value; } }
		}

		public virtual string GetDescription()
		{
			if (integerReference == null)
			{
				return integerValue.ToString();
			}
			else
			{
				return integerReference.key;
			}
		}
	}

}