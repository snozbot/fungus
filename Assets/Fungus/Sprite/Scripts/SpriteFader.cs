/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 * Transitions a sprite from its current color to a target color.
	 * An offset can be applied to slide the sprite in while changing color.
	 */
	[RequireComponent (typeof (SpriteRenderer))]
	public class SpriteFader : MonoBehaviour 
	{
		protected float fadeDuration;
		protected float fadeTimer;
		protected Color startColor;
		protected Color endColor;
		protected Vector2 slideOffset;
		protected Vector3 endPosition;

		protected SpriteRenderer spriteRenderer;

		protected Action onFadeComplete;

		/** 
		 * Attaches a SpriteFader component to a sprite object to transition its color over time.
		 */
		public static void FadeSprite(SpriteRenderer spriteRenderer, Color targetColor, float duration, Vector2 slideOffset, Action onComplete = null)
		{
			if (spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null");
				return;
			}

			// Fade child sprite renderers
			SpriteRenderer[] children = spriteRenderer.gameObject.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer child in children)
			{
				if (child == spriteRenderer)
				{
					continue;
				}
				
				FadeSprite(child, targetColor, duration, slideOffset);
			}

			// Destroy any existing fader component
			SpriteFader oldSpriteFader = spriteRenderer.GetComponent<SpriteFader>();
            if (oldSpriteFader != null)
			{
				Destroy(oldSpriteFader);
			}

			// Early out if duration is zero
			if (duration == 0f)
			{
				spriteRenderer.color = targetColor;
				if (onComplete != null)
				{
					onComplete();
				}
				return;
			}

			// Set up color transition to be applied during update
			SpriteFader spriteFader = spriteRenderer.gameObject.AddComponent<SpriteFader>();
			spriteFader.fadeDuration = duration;
			spriteFader.startColor = spriteRenderer.color;
			spriteFader.endColor = targetColor;
			spriteFader.endPosition = spriteRenderer.transform.position;
			spriteFader.slideOffset = slideOffset;
			spriteFader.onFadeComplete = onComplete;
		}

		protected virtual void Start()
		{
			spriteRenderer = GetComponent<Renderer>() as SpriteRenderer;
		}

		protected virtual void Update() 
		{
			fadeTimer += Time.deltaTime;
			if (fadeTimer > fadeDuration)
			{
				// Snap to final values
				spriteRenderer.color = endColor;
				if (slideOffset.magnitude > 0)
				{
					transform.position = endPosition;
				}

				// Remove this component when transition is complete
				Destroy(this);

				if (onFadeComplete != null)
				{
					onFadeComplete();
				}
			}
			else
			{
				float t = Mathf.SmoothStep(0, 1, fadeTimer / fadeDuration);
				spriteRenderer.color = Color.Lerp(startColor, endColor, t);
				if (slideOffset.magnitude > 0)
				{
					Vector3 startPosition = endPosition;
					startPosition.x += slideOffset.x;
					startPosition.y += slideOffset.y;
					transform.position = Vector3.Lerp(startPosition, endPosition, t);
				}
			}
		}		
	}
}
