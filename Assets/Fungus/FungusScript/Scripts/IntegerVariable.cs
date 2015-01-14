using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class IntegerVariable : Variable 
	{
		public int value;

		protected int startValue;

		public override void OnReset()
		{
			value = startValue;
		}

		public override string ToString()
		{
			return value.ToString();
		}

		protected virtual void Start()
		{
			startValue = value;
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