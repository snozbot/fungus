using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class StringVariable : Variable 
	{
		protected string stringValue;

		public string Value
		{
			get { return (scope == VariableScope.Local) ? stringValue : GlobalVariables.GetString(key); }
			set { if (scope == VariableScope.Local) { stringValue = value; } else { GlobalVariables.SetString(key, value); } }
		}
	}

	[System.Serializable]
	public class StringData
	{
		[SerializeField]
		protected StringVariable stringReference;

		[SerializeField]
		protected string stringValue;

		public string Value
		{
			get { return (stringReference == null) ? stringValue : stringReference.Value; }
			set { if (stringReference == null) { stringValue = value; } else { stringReference.Value = value; } }
		}

		public virtual string GetDescription()
		{
			if (stringReference == null)
			{
				return stringValue;
			}
			else
			{
				return stringReference.key;
			}
		}
	}

}