using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class FloatVariable : Variable 
	{
		public float value;

		protected float startValue;

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
	public struct FloatData
	{
		[SerializeField]
		public FloatVariable floatRef;

		[SerializeField]
		public float floatVal;

		public float Value
		{
			get { return (floatRef == null) ? floatVal : floatRef.value; }
			set { if (floatRef == null) { floatVal = value; } else { floatRef.value = value; } }
		}

		public string GetDescription()
		{
			if (floatRef == null)
			{
				return floatVal.ToString();
			}
			else
			{
				return floatRef.key;
			}
		}
	}

}