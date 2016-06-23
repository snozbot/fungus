/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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

		[Tooltip("Mouse texture to use when hovering mouse over object")]
		public Texture2D hoverCursor;

		protected virtual void OnMouseDown()
		{
			if (!clickEnabled)
			{
				return;
			}

			if (EventSystem.current.IsPointerOverGameObject())
			{
				return; // Ignore this mouse event, pointer is over UI
			}

			// TODO: Cache these object for faster lookup
			ObjectClicked[] handlers = GameObject.FindObjectsOfType<ObjectClicked>();
			foreach (ObjectClicked handler in handlers)
			{
				handler.OnObjectClicked(this);
			}
		}

		protected virtual void OnMouseEnter()
		{
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return; // Ignore this mouse event, pointer is over UI
			}

			changeCursor(hoverCursor);
		}

		protected virtual void OnMouseExit()
		{
			// Always reset the mouse cursor to be on the safe side
			SetMouseCursor.ResetMouseCursor();
		}

		protected virtual void changeCursor(Texture2D cursorTexture)
		{
			if (!clickEnabled)
			{
				return;
			}

			Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		}
	}

}
