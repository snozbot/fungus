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
		public IntegerVariable integerReference;
		public int integerValue;
	}

}