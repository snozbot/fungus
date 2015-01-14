using UnityEngine;
using System.Collections;

namespace Fungus
{

	public class StringVariable : Variable 
	{
		public string value = "";

		protected string startValue = "";

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
	public struct StringData
	{
		[SerializeField]
		public StringVariable stringRef;

		[SerializeField]
		public string stringVal;

		public string Value
		{
			get { return (stringRef == null) ? stringVal : stringRef.value; }
			set { if (stringRef == null) { stringVal = value; } else { stringRef.value = value; } }
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