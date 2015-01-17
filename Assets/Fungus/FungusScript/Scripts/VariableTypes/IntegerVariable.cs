using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("", "Integer")]
	[AddComponentMenu("")]
	public class IntegerVariable : VariableBase<int> 
	{}

	[System.Serializable]
	public struct IntegerData
	{
		[SerializeField]
		public IntegerVariable integerRef;

		[SerializeField]
		public int integerVal;

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