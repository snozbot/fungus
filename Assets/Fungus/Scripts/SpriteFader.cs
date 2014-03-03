using UnityEngine;
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
		float fadeDuration;
		float fadeTimer;
		Color startColor;
		Color endColor;
		Vector2 slideOffset;
		Vector3 endPosition;

		SpriteRenderer spriteRenderer;

		/** 
		 * Attaches a SpriteFader component to a sprite object to transition its color over time.
		 */
		public static void FadeSprite(SpriteRenderer spriteRenderer, Color targetColor, float duration, Vector2 slideOffset)
		{
			if (spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null");
				return;
			}

			// Destroy any existing fader component
			SpriteFader oldSpriteFader = spriteRenderer.GetComponent<SpriteFader>();
			{
				Destroy(oldSpriteFader);
			}

			// Early out if duration is zero
			if (duration == 0f)
			{
				spriteRenderer.color = targetColor;
				return;
			}

			// Set up color transition to be applied during update
			SpriteFader spriteFader = spriteRenderer.gameObject.AddComponent<SpriteFader>();
			spriteFader.fadeDuration = duration;
			spriteFader.startColor = spriteRenderer.color;
			spriteFader.endColor = targetColor;
			spriteFader.endPosition = spriteRenderer.transform.position;
			spriteFader.slideOffset = slideOffset;

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
		}

		void Start()
		{
			spriteRenderer = renderer as SpriteRenderer;
		}

		void Update() 
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
