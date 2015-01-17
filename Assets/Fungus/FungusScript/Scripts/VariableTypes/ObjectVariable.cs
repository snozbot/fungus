using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Object")]
	[AddComponentMenu("")]
	public class ObjectVariable : VariableBase<Object>
	{}

	[System.Serializable]
	public struct ObjectData
	{
		[SerializeField]
		public ObjectVariable objectRef;
		
		[SerializeField]
		public Object objectVal;
		
		public Object Value
		{
			get { return (objectRef == null) ? objectVal : objectRef.value; }
			set { if (objectRef == null) { objectVal = value; } else { objectRef.value = value; } }
		}

		public string GetDescription()
		{
			if (objectRef == null)
			{
				return objectVal.ToString();
			}
			else
			{
				return objectRef.key;
			}
		}
	}

}