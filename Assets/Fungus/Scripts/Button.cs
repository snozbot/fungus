using UnityEngine;
using System;
using System.Collections;
using Fungus;

namespace Fungus
{
	/**
	 * Button click handler class for making sprites clickable.
	 * When the user taps on the sprite, an Action delegate method is called
	 */
	[RequireComponent (typeof (SpriteRenderer))]
	public class Button : MonoBehaviour 
	{
		/**
		 *  Delegate method to call when the player clicks the button.
		 */
		public Action buttonAction;

		/**
		 * Automatically hides the button when displaying story text/options or waiting.
		 */
		public bool autoHide;

		/**
		 * Automatically hides the button when the specified game value is set (i.e. not equal to zero).
		 */
		public string hideOnSetValue;

		float targetAlpha;
		bool showButton;

		/**
		 * Set visibility of a button object and set the delegate method to call when clicked.
		 * If the object does not already have a Collider2D component, then a BoxCollider2D is added.
		 * @param _visible Setting this to true makes the button visible, unless overridden by property settings
		 * @param _buttonAction An Action delegate method to call when the player clicks on the button
		 */
		public void Show(bool _visible, Action _buttonAction)
		{
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			if (spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null");
				return;
			}

			// Add a BoxCollider2d if none currently exists
			if (collider2D == null)
			{
				gameObject.AddComponent<BoxCollider2D>();
			}

			showButton = _visible;
			buttonAction = _buttonAction;

			if (_visible)
			{
				targetAlpha = 1f;
			}
			else
			{
				targetAlpha = 0f;
			}

			UpdateTargetAlpha();
		}

		public void SetAlpha(float alpha)
		{
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			Color color = spriteRenderer.color;
			color.a = alpha;
			spriteRenderer.color = color;
		}

		void UpdateTargetAlpha()
		{
			// Automatically display button when game is in idle state (not displaying story text/options or waiting)
			if (autoHide)
			{
				if (showButton &&
				    Game.GetInstance().IsGameIdle())
				{
					targetAlpha = 1f;

				}
				else
				{
					targetAlpha = 0f;
				}
			}

			// Hide the button if the specified game value is non-zero
			if (hideOnSetValue.Length > 0 &&
			    Game.GetInstance().GetValue(hideOnSetValue) != 0)
			{
				targetAlpha = 0f;
			}
		}

		void Update()
		{
			UpdateTargetAlpha();

			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			Color color = spriteRenderer.color;
			float fadeSpeed = (1f / Game.GetInstance().buttonFadeDuration);
			color.a = Mathf.MoveTowards(color.a, targetAlpha, Time.deltaTime * fadeSpeed);
			spriteRenderer.color = color;
		}

		void OnMouseUpAsButton() 
		{
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;

			// Ignore button press if sprite is not fully visible
			if (spriteRenderer.color.a != 1f)
			{
				return;
			}

			// Sound effect
			Game.GetInstance().PlayButtonClick();

			CommandQueue commandQueue = Game.GetInstance().commandQueue;		
			commandQueue.CallCommandMethod(buttonAction);
		}
	}
}