using UnityEngine;
using System.Collections;

namespace Fungus
{
	[VariableInfo("Other", "Sprite")]
	[AddComponentMenu("")]
	public class SpriteVariable : VariableBase<Sprite>
	{}

	[System.Serializable]
	public struct SpriteData
	{
		[SerializeField]
		public SpriteVariable spriteRef;
		
		[SerializeField]
		public Sprite spriteVal;
		
		public Sprite Value
		{
			get { return (spriteRef == null) ? spriteVal : spriteRef.value; }
			set { if (spriteRef == null) { spriteVal = value; } else { spriteRef.value = value; } }
		}

		public string GetDescription()
		{
			if (spriteRef == null)
			{
				return spriteVal.ToString();
			}
			else
			{
				return spriteRef.key;
			}
		}
	}

}