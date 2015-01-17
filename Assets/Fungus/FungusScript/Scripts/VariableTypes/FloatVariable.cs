using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("", "Float")]
	[AddComponentMenu("")]
	public class FloatVariable : VariableBase<float>
	{}

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