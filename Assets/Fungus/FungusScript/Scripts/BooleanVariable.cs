using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	public class BooleanVariable : Variable 
	{
		public bool value;

		protected bool startValue;

		public override void OnReset()
		{
			value = startValue;
		}

		public override string ToString()
		{
			return value.ToString();
		}

		protected virtual void Start()
		{
			startValue = value;
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