using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	public class StringVariable : FungusVariable 
	{
		string stringValue;

		public string Value
		{
			get { return (scope == VariableScope.Local) ? stringValue : Variables.GetString(key); }
			set { if (scope == VariableScope.Local) { stringValue = value; } else { Variables.SetString(key, value); } }
		}
	}

	[System.Serializable]
	public class StringData
	{
		public StringVariable stringReference;
		public string stringValue;

		public string Value
		{
			get { return (stringReference == null) ? stringValue : stringReference.Value; }
			set { if (stringReference == null) { stringValue = value; } else { stringReference.Value = value; } }
		}
	}

}