using UnityEngine;
using System.Collections;

namespace Fungus
{

	[VariableInfo("", "String")]
	[AddComponentMenu("")]
	public class StringVariable : VariableBase<string>
	{}

	[System.Serializable]
	public class StringData
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