using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Color")]
	[AddComponentMenu("")]
	public class ColorVariable : VariableBase<Color>
	{}

	[System.Serializable]
	public struct ColorData
	{
		[SerializeField]
		public ColorVariable colorRef;
		
		[SerializeField]
		public Color colorVal;
		
		public Color Value
		{
			get { return (colorRef == null) ? colorVal : colorRef.value; }
			set { if (colorRef == null) { colorVal = value; } else { colorRef.value = value; } }
		}

		public string GetDescription()
		{
			if (colorRef == null)
			{
				return colorVal.ToString();
			}
			else
			{
				return colorRef.key;
			}
		}
	}

}