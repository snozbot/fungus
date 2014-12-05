using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class StringVariable : Variable 
	{
		protected string stringVal;

		public string Value
		{
			get { return (scope == VariableScope.Local) ? stringVal : GlobalVariables.GetString(key); }
			set { if (scope == VariableScope.Local) { stringVal = value; } else { GlobalVariables.SetString(key, value); } }
		}

		public override void OnReset()
		{
			Value = "";
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[System.Serializable]
	public struct StringData
	{
		[SerializeField]
		public StringVariable stringRef;

		[SerializeField]
		public string stringVal;

		public string Value
		{
			get { return (stringRef == null) ? stringVal : stringRef.Value; }
			set { if (stringRef == null) { stringVal = value; } else { stringRef.Value = value; } }
		}

		public string GetDescription()
		{
			if (stringRef == null)
			{
				return stringVal;
			}
			else
			{
				return stringRef.key;
			}
		}
	}

}