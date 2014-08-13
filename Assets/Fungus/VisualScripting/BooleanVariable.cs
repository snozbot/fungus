using UnityEngine;
using System;
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

	[System.Serializable]
	public class BooleanData
	{
		[SerializeField]
		BooleanVariable booleanReference;

		[SerializeField]
		bool booleanValue;

		public bool Value
		{
			get { return (booleanReference == null) ? booleanValue : booleanReference.Value; }
			set { if (booleanReference == null) { booleanValue = value; } else { booleanReference.Value = value; } }
		}

		public string GetDescription()
		{
			if (booleanReference == null)
			{
				return booleanValue.ToString();
			}
			else
			{
				return booleanReference.key;
			}
		}
	}

}