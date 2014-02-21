using UnityEngine;
using System.Collections;
using Fungus;

namespace Fungus
{
	// Simple button handler class.
	// When the user taps on the button, the named method is called on ancestor game objects (if it exists).
	[RequireComponent (typeof (SpriteController))]
	[RequireComponent (typeof (Collider2D))]
	public class Button : MonoBehaviour 
	{
		public string methodName;

		public SpriteController spriteController;

		public bool autoDisable = false;

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

			Room room = Game.GetInstance().activeRoom;
			if (room == null)
			{
				return;
			}

			room.ExecuteCommandMethod(methodName);

			if (autoDisable)
			{
				gameObject.SetActive(false);
			}
		}
	}
}