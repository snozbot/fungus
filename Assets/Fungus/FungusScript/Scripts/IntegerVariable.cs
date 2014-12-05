using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class IntegerVariable : Variable 
	{
		protected int integerVal;

		public int Value
		{
			get { return (scope == VariableScope.Local) ? integerVal : GlobalVariables.GetInteger(key); }
			set { if (scope == VariableScope.Local) { integerVal = value; } else { GlobalVariables.SetInteger(key, value); } }
		}

		public override void OnReset()
		{
			Value = 0;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[System.Serializable]
	public struct IntegerData
	{
		[SerializeField]
		public IntegerVariable integerRef;

		[SerializeField]
		public int integerVal;

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
				return integerRef.key;
			}
		}
	}

}