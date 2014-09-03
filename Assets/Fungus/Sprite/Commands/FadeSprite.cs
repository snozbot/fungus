using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandInfo("Sprite", 
	             "Fade Sprite", 
	             "Fades a sprite to a target color over a period of time.", 
	             212, 178, 211)]
	public class FadeSprite : FungusCommand 
	{
		public SpriteRenderer spriteRenderer;
		public float duration;
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
	}

}