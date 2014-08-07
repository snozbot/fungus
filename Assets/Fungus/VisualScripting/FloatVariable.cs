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

}