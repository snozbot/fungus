using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Fungus
{
	/**
	 * Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
	 * The Game Object must have a Collider2D component attached.
	 * Use in conjunction with the ObjectClicked Flowchart event handler.
	 */
	public class Clickable2D : MonoBehaviour
	{
		[Tooltip("Is object clicking enabled")]
		public bool clickEnabled = true;

		public Texture2D hoverOverCursorTexture;

		void Start() {
			// TODO: Decide on how best to write this
			// The below will set the hover over cursor for all Clickable2D
			// hoverOverCursorTexture = Resources.Load("Sprite/mouse-cursor") as Texture2D;
		}

		void OnMouseDown()
		{
			if (!clickEnabled)
			{
				return;
			}

			// TODO: Cache these object for faster lookup
			ObjectClicked[] handlers = GameObject.FindObjectsOfType<ObjectClicked>();
			foreach (ObjectClicked handler in handlers)
			{
				handler.OnObjectClicked(this);
			}
		}

		void OnMouseEnter()
		{
			changeCursor(hoverOverCursorTexture);
		}

		void OnMouseExit()
		{
			changeCursor(null);
		}

		void changeCursor(Texture2D cursorTexture)
		{
			if (!clickEnabled)
			{
				return;
			}

			Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		}
	}

}
