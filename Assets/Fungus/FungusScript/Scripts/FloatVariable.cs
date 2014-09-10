using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class FloatVariable : Variable 
	{
		float floatValue;

		public float Value
		{
			get { return (scope == VariableScope.Local) ? floatValue : GlobalVariables.GetFloat(key); }
			set { if (scope == VariableScope.Local) { floatValue = value; } else {	GlobalVariables.SetFloat(key, value); } }
		}
	}

	[System.Serializable]
	public class FloatData
	{
		[SerializeField]
		FloatVariable floatReference;

		[SerializeField]
		float floatValue;

		public float Value
		{
			get { return (floatReference == null) ? floatValue : floatReference.Value; }
			set { if (floatReference == null) { floatValue = value; } else { floatReference.Value = value; } }
		}

		public string GetDescription()
		{
			if (floatReference == null)
			{
				return floatValue.ToString();
			}
			else
			{
				return floatReference.key;
			}
		}
	}

}