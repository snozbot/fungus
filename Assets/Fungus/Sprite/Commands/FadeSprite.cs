using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Fade Sprite", 
	             "Fades a sprite to a target color over a period of time.")]
	public class FadeSprite : Command 
	{
		public SpriteRenderer spriteRenderer;
		public float duration = 1f;
		public Color targetColor = Color.white;
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			CameraController cameraController = CameraController.GetInstance();

			if (waitUntilFinished)
			{
				cameraController.waiting = true;
			}

			SpriteFader.FadeSprite(spriteRenderer, targetColor, duration, Vector2.zero, delegate {
				if (waitUntilFinished)
				{
					cameraController.waiting = false;
					Continue();
				}
			});

			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		public override string GetSummary()
		{
			if (spriteRenderer == null)
			{
				return "Error: No sprite renderer selected";
			}

			return spriteRenderer.name + " to " + targetColor.ToString();
		}

		public override Color GetButtonColor()
		{
			return new Color32(221, 184, 169, 255);
		}
	}

}