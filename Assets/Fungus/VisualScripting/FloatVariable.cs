using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	public class FloatVariable : FungusVariable 
	{
		float floatValue;

		public float Value
		{
			get { return (scope == VariableScope.Local) ? floatValue : Variables.GetFloat(key); }
			set { if (scope == VariableScope.Local) { floatValue = value; } else {	Variables.SetFloat(key, value); } }
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