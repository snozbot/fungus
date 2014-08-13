using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	public class IntegerVariable : FungusVariable 
	{
		int integerValue;

		public int Value
		{
			get { return (scope == VariableScope.Local) ? integerValue : Variables.GetInteger(key); }
			set { if (scope == VariableScope.Local) { integerValue = value; } else { Variables.SetInteger(key, value); } }
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