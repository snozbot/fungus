/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Transform")]
	[AddComponentMenu("")]
    [System.Serializable]
	public class TransformVariable : VariableBase<Transform>
	{}

	[System.Serializable]
	public struct TransformData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(TransformVariable))]
		public TransformVariable transformRef;
		
		[SerializeField]
		public Transform transformVal;

		public TransformData(Transform v)
		{
			transformVal = v;
			transformRef = null;
		}
		
		public static implicit operator Transform(TransformData vector3Data)
		{
			return vector3Data.Value;
		}

		public Transform Value
		{
			get { return (transformRef == null) ? transformVal : transformRef.value; }
			set { if (transformRef == null) { transformVal = value; } else { transformRef.value = value; } }
		}

		public string GetDescription()
		{
			if (transformRef == null)
			{
				return transformVal.ToString();
			}
			else
			{
				return transformRef.key;
			}
		}
	}

}