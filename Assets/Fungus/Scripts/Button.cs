using UnityEngine;
using System.Collections;
using Fungus;

namespace Fungus
{
	// Simple button handler class.
	// When the user taps on the button, the named method is called on ancestor game objects (if it exists).
	[RequireComponent (typeof (SpriteRenderer))]
	[RequireComponent (typeof (BoxCollider2D))]
	public class Button : MonoBehaviour 
	{
		public string methodName;

		public SpriteRenderer spriteRenderer;

		public bool autoDisable = false;

		void Start()
		{
			spriteRenderer = GetComponent<SpriteRenderer>();
		}

		void OnMouseUpAsButton() 
		{
			if (methodName == "")
			{
				return;
			}

			// Ignore button press if button is not fully visible
			if (spriteRenderer.color.a != 1f)
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