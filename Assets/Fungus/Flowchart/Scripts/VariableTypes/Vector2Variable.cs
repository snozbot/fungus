using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Vector2")]
	[AddComponentMenu("")]
	public class Vector2Variable : VariableBase<Vector2>
	{}

	[System.Serializable]
	public struct Vector2Data
	{
		[SerializeField]
		public Vector2Variable vector2Ref;
		
		[SerializeField]
		public Vector2 vector2Val;
		
		public Vector2 Value
		{
			get { return (vector2Ref == null) ? vector2Val : vector2Ref.value; }
			set { if (vector2Ref == null) { vector2Val = value; } else { vector2Ref.value = value; } }
		}

		public string GetDescription()
		{
			if (vector2Ref == null)
			{
				return vector2Val.ToString();
			}
			else
			{
				return vector2Ref.key;
			}
		}
	}

}