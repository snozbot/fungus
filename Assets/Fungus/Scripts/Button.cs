using UnityEngine;
using System;
using System.Collections;
using Fungus;

namespace Fungus
{
	// Simple button handler class.
	// When the user taps on the button, the named method is called on ancestor game objects (if it exists).
	[RequireComponent (typeof (SpriteRenderer))]
	[RequireComponent (typeof (Collider2D))]
	public class Button : MonoBehaviour 
	{
		public Action buttonAction;
		public SpriteRenderer spriteRenderer;

		// Makes a sprite clickable by attaching a Button component (and BoxCollider2D if required)
		public static void MakeButton(SpriteRenderer _spriteRenderer, Action _buttonAction)
		{
			if (_spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null");
				return;
			}

			// Remove any previous button component that was added
			Button oldButton = _spriteRenderer.gameObject.GetComponent<Button>();
			if (oldButton != null)
			{
				Destroy(oldButton);
			}

			// Add a BoxCollider2d if none currently exists
			if (_spriteRenderer.gameObject.GetComponent<Collider2D>() == null)
			{
				_spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
			}

			Button button = _spriteRenderer.gameObject.AddComponent<Button>();
			button.buttonAction = _buttonAction;
			button.spriteRenderer = _spriteRenderer;
		}

		void OnMouseUpAsButton() 
		{
			// Ignore button press if sprite is not fully visible
			if (spriteRenderer.color.a != 1f)
			{
				return;
			}

			CommandQueue commandQueue = Game.GetInstance().commandQueue;		
			commandQueue.CallCommandMethod(buttonAction);
		}
	}
}