using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Vector3")]
	[AddComponentMenu("")]
	public class Vector3Variable : VariableBase<Vector3>
	{}

	[System.Serializable]
	public struct Vector3Data
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(Vector3Variable))]
		public Vector3Variable vector3Ref;
		
		[SerializeField]
		public Vector3 vector3Val;

		public Vector3Data(Vector3 v)
		{
			vector3Val = v;
			vector3Ref = null;
		}
		
		public static implicit operator Vector3(Vector3Data vector3Data)
		{
			return vector3Data.Value;
		}

		public Vector3 Value
		{
			get { return (vector3Ref == null) ? vector3Val : vector3Ref.value; }
			set { if (vector3Ref == null) { vector3Val = value; } else { vector3Ref.value = value; } }
		}

		public string GetDescription()
		{
			if (vector3Ref == null)
			{
				return vector3Val.ToString();
			}
			else
			{
				return vector3Ref.key;
			}
		}
	}

}