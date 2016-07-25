/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Material")]
	[AddComponentMenu("")]
    [System.Serializable]
	public class MaterialVariable : VariableBase<Material>
	{}

	[System.Serializable]
	public struct MaterialData
	{
		[SerializeField]
		[VariableProperty("<Value>", typeof(MaterialVariable))]
		public MaterialVariable materialRef;
		
		[SerializeField]
		public Material materialVal;

		public MaterialData(Material v)
		{
			materialVal = v;
			materialRef = null;
		}
		
		public static implicit operator Material(MaterialData materialData)
		{
			return materialData.Value;
		}

		public Material Value
		{
			get { return (materialRef == null) ? materialVal : materialRef.value; }
			set { if (materialRef == null) { materialVal = value; } else { materialRef.value = value; } }
		}

		public string GetDescription()
		{
			if (materialRef == null)
			{
				return materialVal.ToString();
			}
			else
			{
				return materialRef.key;
			}
		}
	}

}