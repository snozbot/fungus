using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	public class BooleanVariable : Variable 
	{
		protected bool booleanVal;

		public bool Value
		{
			get { return (scope == VariableScope.Local) ? booleanVal : GlobalVariables.GetBoolean(key); }
			set { if (scope == VariableScope.Local) { booleanVal = value; } else { GlobalVariables.SetBoolean(key, value); } }
		}

		public override void OnReset()
		{
			Value = false;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	[System.Serializable]
	public struct BooleanData
	{
		[SerializeField]
		public BooleanVariable booleanRef;

		[SerializeField]
		public bool booleanVal;

		public bool Value
		{
			get { return (booleanRef == null) ? booleanVal : booleanRef.Value; }
			set { if (booleanRef == null) { booleanVal = value; } else { booleanRef.Value = value; } }
		}

		public string GetDescription()
		{
			if (booleanRef == null)
			{
				return booleanVal.ToString();
			}
			else
			{
				return booleanRef.key;
			}
		}
	}

}