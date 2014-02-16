using UnityEngine;
using System.Collections;

namespace Fungus
{
	// Simple button handler class.
	// When the user taps on the button, the named method is called on ancestor game objects (if it exists).
	[RequireComponent (typeof (SpriteController))]
	[RequireComponent (typeof (Collider2D))]
	public class Button : MonoBehaviour 
	{
		public string methodName;

		public bool autoDisable = true;

		SpriteController spriteController;

		void Start()
		{
			spriteController = GetComponent<SpriteController>();
		}

		void OnMouseUpAsButton() 
		{
			if (methodName == "")
			{
				return;
			}

			// Ignore button press if button is not fully visible
			if (!spriteController.isShown)
			{
				return;
			}

			Game game = Game.GetInstance();

			game.ResetCommandQueue();
			gameObject.SendMessageUpwards(methodName, SendMessageOptions.RequireReceiver);
			game.ExecuteCommandQueue();

			if (autoDisable)
			{
				gameObject.SetActive(false);
			}
		}
	}
}