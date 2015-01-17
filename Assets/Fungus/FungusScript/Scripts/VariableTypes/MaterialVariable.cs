using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Material")]
	[AddComponentMenu("")]
	public class MaterialVariable : VariableBase<Material>
	{}

	[System.Serializable]
	public struct MaterialData
	{
		[SerializeField]
		public MaterialVariable materialRef;
		
		[SerializeField]
		public Material materialVal;
		
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