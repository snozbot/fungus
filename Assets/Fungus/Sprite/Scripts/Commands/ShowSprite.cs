/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Show Sprite", 
	             "Makes a sprite visible / invisible by setting the color alpha.")]
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class ShowSprite : Command
	{
		[Tooltip("Sprite object to be made visible / invisible")]
		public SpriteRenderer spriteRenderer;

		[Tooltip("Make the sprite visible or invisible")]
		public BooleanData _visible = new BooleanData(false);

		[Tooltip("Affect the visibility of child sprites")]
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
						SetSpriteAlpha(sr, _visible.Value);
					}
				}
				else
				{
					SetSpriteAlpha(spriteRenderer, _visible.Value);
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

			return spriteRenderer.name + " to " + (_visible.Value ? "visible" : "invisible");
		}

		public override Color GetButtonColor()
		{
			return new Color32(221, 184, 169, 255);
		}

		#region Backwards compatibility

		[HideInInspector] [FormerlySerializedAs("visible")] public bool visibleOLD;

		protected virtual void OnEnable()
		{
			if (visibleOLD != default(bool))
			{
				_visible.Value = visibleOLD;
				visibleOLD = default(bool);
			}
		}

		#endregion
	}

}