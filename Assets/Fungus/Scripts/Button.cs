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
		[Tooltip("Automatically hides the button when displaying story text/options or waiting.")]
		public bool autoHide = true;

		/**
		 * Automatically hides the button when the named boolean variable is set using SetBoolean()
		 */
		[Tooltip("Automatically hides the button when the named boolean variable is set using the SetBoolean() command.")]
		public string hideOnBoolean;

		/**
		 * Sound effect to play when button is clicked.
		 */
		[Tooltip("Sound effect to play when button is clicked.")]
		public AudioClip clickSound;

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
				    Game.GetInstance().GetShowAutoButtons())
				{
					targetAlpha = 1f;

				}
				else
				{
					targetAlpha = 0f;
				}
			}

			// Hide the button if the specified boolean variable is true
			if (hideOnBoolean.Length > 0 &&
			    Variables.GetBoolean(hideOnBoolean))
			{
				targetAlpha = 0f;
			}
		}

		void Update()
		{
			UpdateTargetAlpha();

			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;
			float fadeSpeed = (1f / Game.GetInstance().buttonFadeDuration);

			float alpha = Mathf.MoveTowards(spriteRenderer.color.a, targetAlpha, Time.deltaTime * fadeSpeed);;

			// Set alpha for this sprite and any child sprites
			SpriteRenderer[] children = spriteRenderer.gameObject.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer child in children)
			{
				Color color = child.color;
				color.a = alpha;
				child.color = color;
			}
		}

		void OnMouseUpAsButton() 
		{
			SpriteRenderer spriteRenderer = renderer as SpriteRenderer;

			// Ignore button press if sprite is not fully visible or 
			// if the game is not in an idle state
			if (spriteRenderer.color.a != 1f ||
			    !Game.GetInstance().GetShowAutoButtons())
			{
				return;
			}

			// Click sound effect
			if (clickSound != null)
			{
				AudioSource.PlayClipAtPoint(clickSound, Vector3.zero);
			}

			CommandQueue commandQueue = Game.GetInstance().commandQueue;		
			commandQueue.CallCommandMethod(buttonAction);
		}
	}
}