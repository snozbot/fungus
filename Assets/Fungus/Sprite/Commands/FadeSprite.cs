using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandName("Fade Sprite")]
	[HelpText("Fades a sprite to a target color over a period of time.")]
	public class FadeSprite : FungusCommand 
	{
		public float duration;
		public SpriteRenderer spriteRenderer;
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
	}

}