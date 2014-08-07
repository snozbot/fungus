using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	public class BooleanVariable : FungusVariable 
	{
		bool booleanValue;

		public bool Value
		{
			get { return (scope == VariableScope.Local) ? booleanValue : Variables.GetBoolean(key); }
			set { if (scope == VariableScope.Local) { booleanValue = value; } else { Variables.SetBoolean(key, value); } }
		}
	}

}