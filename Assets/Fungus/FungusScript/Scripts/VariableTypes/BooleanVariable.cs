using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	[VariableInfo("", "Boolean")]
	[AddComponentMenu("")]
	public class BooleanVariable : VariableBase<bool>
	{}

	[System.Serializable]
	public class BooleanData
	{
		[SerializeField]
		public BooleanVariable booleanRef;

		[SerializeField]
		public bool booleanVal;

		public bool Value
		{
			get { return (booleanRef == null) ? booleanVal : booleanRef.value; }
			set { if (booleanRef == null) { booleanVal = value; } else { booleanRef.value = value; } }
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