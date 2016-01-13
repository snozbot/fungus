using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Show Sprite", 
	             "Makes a sprite visible / invisible by setting the color alpha.")]
	[AddComponentMenu("")]
	public class ShowSprite : Command 
	{
		[Tooltip("Sprite object to be made visible / invisible")]
		public SpriteRenderer spriteRenderer;

		[Tooltip("Make the sprite visible or invisible")]
		public bool visible = true;

		public bool affectChildren = true;

		public override void OnEnter()
		{
			if (spriteRenderer != null)
			{
				if (affectChildren)
				{
					SpriteRenderer[] children = spriteRenderer.gameObject.GetComponentsInChildren<SpriteRenderer>();
					foreach (SpriteRenderer sr in children)
					{
						SetSpriteAlpha(sr, visible);
					}
				}
				else
				{
					SetSpriteAlpha(spriteRenderer, visible);
				}
			}

			Continue();
		}

		protected virtual void SetSpriteAlpha(SpriteRenderer renderer, bool visible)
		{
			Color spriteColor = renderer.color;
			spriteColor.a = visible ? 1f : 0f;
			renderer.color = spriteColor;
		}

		public override string GetSummary()
		{
			if (spriteRenderer == null)
			{
				return "Error: No sprite renderer selected";
			}

			return spriteRenderer.name + " to " + (visible ? "visible" : "invisible");
		}

		public override Color GetButtonColor()
		{
			return new Color32(221, 184, 169, 255);
		}
	}

}