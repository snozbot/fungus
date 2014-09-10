using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class IntegerVariable : Variable 
	{
		int integerValue;

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
		IntegerVariable integerReference;

		[SerializeField]
		int integerValue;

		public int Value
		{
			get { return (integerReference == null) ? integerValue : integerReference.Value; }
			set { if (integerReference == null) { integerValue = value; } else { integerReference.Value = value; } }
		}

		public string GetDescription()
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