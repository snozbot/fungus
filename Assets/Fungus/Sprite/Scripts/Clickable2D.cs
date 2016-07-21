/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
	/**
	 * Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
	 * The Game Object must have a Collider2D component attached.
	 * Use in conjunction with the ObjectClicked Flowchart event handler.
	 */
	public class Clickable2D : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
	{
		[Tooltip("Is object clicking enabled")]
		public bool clickEnabled = true;

		[Tooltip("Mouse texture to use when hovering mouse over object")]
		public Texture2D hoverCursor;

        protected virtual void Start()
        {
            // If the main camera doesn't already have a Physics2DRaycaster then add one automatically to
            // use UI raycasts for hit detection. This allows UI to block clicks on objects behind.
            if (Camera.main == null)
                return;

            var raycast = Camera.main.GetComponent<Physics2DRaycaster>();
            if (raycast == null)
            {
                Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
            }
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

        #region IPointerClickHandler implementation
        public void OnPointerClick(PointerEventData eventData)
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
        #endregion

        #region IPointerEnterHandler
        public void OnPointerEnter(PointerEventData eventData)
        {
            changeCursor(hoverCursor);
        }
        #endregion
	}

}
