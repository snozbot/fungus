using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Texture")]
	public class TextureVariable : VariableBase<Texture>
	{}

	[System.Serializable]
	public class TextureData
	{
		[SerializeField]
		public TextureVariable textureRef;
		
		[SerializeField]
		public Texture textureVal;
		
		public Texture Value
		{
			get { return (textureRef == null) ? textureVal : textureRef.value; }
			set { if (textureRef == null) { textureVal = value; } else { textureRef.value = value; } }
		}

		public string GetDescription()
		{
			if (textureRef == null)
			{
				return textureVal.ToString();
			}
			else
			{
				return textureRef.key;
			}
		}
	}

}