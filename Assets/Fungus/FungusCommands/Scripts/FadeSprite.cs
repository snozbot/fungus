using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{

	public class FadeSprite : FungusCommand 
	{
		public float duration;
		public SpriteRenderer spriteRenderer;
		public Color targetColor = Color.white;
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			Game game = Game.GetInstance();

			if (waitUntilFinished)
			{
				game.waiting = true;
			}

			SpriteFader.FadeSprite(spriteRenderer, targetColor, duration, Vector2.zero, delegate {
				if (waitUntilFinished)
				{
					game.waiting = false;
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