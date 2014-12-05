using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class FloatVariable : Variable 
	{
		protected float floatVal;

		public float Value
		{
			get { return (scope == VariableScope.Local) ? floatVal : GlobalVariables.GetFloat(key); }
			set { if (scope == VariableScope.Local) { floatVal = value; } else {	GlobalVariables.SetFloat(key, value); } }
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
	public struct FloatData
	{
		[SerializeField]
		public FloatVariable floatRef;

		[SerializeField]
		public float floatVal;

		public float Value
		{
			get { return (floatRef == null) ? floatVal : floatRef.Value; }
			set { if (floatRef == null) { floatVal = value; } else { floatRef.Value = value; } }
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