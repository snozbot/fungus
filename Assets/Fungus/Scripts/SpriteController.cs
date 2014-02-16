using UnityEngine;
using System.Collections;

namespace Fungus
{
	// Extends sprite functionality with support for fading and playing simple animations
	[RequireComponent (typeof (SpriteRenderer))]
	public class SpriteController : MonoBehaviour 
	{
		[HideInInspector]
		public bool isShown;

		private float spriteAlpha;

		private float fadeDuration;
		private float fadeTimer;
		private float startAlpha;
		private float endAlpha;
		private Vector3 startPosition;
		private Vector3 startSlideOffset;

		void Start() 
		{
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			spriteAlpha = spriteRenderer.color.a;
		}
		
		void Update() 
		{
			if (fadeDuration > 0f)
			{
				fadeTimer += Time.deltaTime;
				if (fadeTimer > fadeDuration)
				{
					spriteAlpha = endAlpha;
					fadeDuration = 0;
				}
				else
				{
					float t = Mathf.SmoothStep(0, 1, fadeTimer / fadeDuration);
					spriteAlpha = Mathf.Lerp(startAlpha, endAlpha, t);
					transform.position = Vector3.Lerp(startPosition + startSlideOffset, startPosition, t);
				}
			}

			UpdateSpriteColor();
		}

		void UpdateSpriteColor()
		{
			isShown = (spriteAlpha == 1f);

			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;

			Color color = spriteRenderer.material.color;
			color.a = spriteAlpha;
			spriteRenderer.material.color = color;

			SpriteRenderer[] children = gameObject.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer child in children)
			{
				Color childColor = child.material.color;
				childColor.a = spriteAlpha;
				child.material.color = childColor;
			}
		}

		public void Fade(float targetAlpha, float duration)
		{
			if (duration == 0f)
			{
				spriteAlpha = targetAlpha;
				return;
			}
			
			fadeDuration = duration;
			fadeTimer = 0;
			
			startAlpha = spriteAlpha;
			endAlpha = targetAlpha;

			startPosition = transform.position;
			startSlideOffset = Vector3.zero;

			SpriteController[] children = gameObject.GetComponentsInChildren<SpriteController>();
			foreach (SpriteController child in children)
			{
				if (child == this)
				{
					continue;
				}

				child.Fade(targetAlpha, duration);
			}

		}

		public void SlideFade(float targetAlpha, float duration, Vector2 slideOffset)
		{
			Fade(targetAlpha, duration);

			if (duration > 0)
			{
				startSlideOffset = slideOffset;
			}
		}

		public void PlayAnimation(string animationName)
		{
			Animator anim = GetComponent<Animator>();
			if (anim == null)
			{
				Debug.LogError("Failed to find animator component when playing animation " + animationName);
				return;
			}

			anim.Play(animationName);
		}
	}
}
