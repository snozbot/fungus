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
     * The Game Object must have a Collider or Collider2D component attached.
     * Use in conjunction with the ObjectClicked Flowchart event handler.
     */
    public class Clickable2D : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Is object clicking enabled")]
        public bool clickEnabled = true;

        [Tooltip("Mouse texture to use when hovering mouse over object")]
        public Texture2D hoverCursor;

        [Tooltip("Use the UI Event System to check for clicks. Clicks that hit an overlapping UI object will be ignored. Camera must have a PhysicsRaycaster component, or a Physics2DRaycaster for 2D colliders.")]
        public bool useEventSystem;

        #region Legacy OnMouseX methods
        protected virtual void OnMouseDown()
        {
            if (!useEventSystem)
            {
                DoPointerClick();
            }
        }

        protected virtual void OnMouseEnter()
        {
            if (!useEventSystem)
            {
                DoPointerEnter();
            }
        }

        protected virtual void OnMouseExit()
        {
            if (!useEventSystem)
            {
                DoPointerExit();
            }
        }
        #endregion

        #region IPointerXHandler implementations
        public void OnPointerClick(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerClick();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerExit();
            }
        }

        protected virtual void DoPointerClick()
        {
            if (!clickEnabled)
            {
                return;
            }
            
            // TODO: Cache these objects for faster lookup
            ObjectClicked[] handlers = GameObject.FindObjectsOfType<ObjectClicked>();
            foreach (ObjectClicked handler in handlers)
            {
                handler.OnObjectClicked(this);
            }
        }

        protected virtual void DoPointerEnter()
        {
            ChangeCursor(hoverCursor);
        }

        protected virtual void DoPointerExit()
        {
            // Always reset the mouse cursor to be on the safe side
            SetMouseCursor.ResetMouseCursor();
        }
        #endregion

        protected virtual void ChangeCursor(Texture2D cursorTexture)
        {
            if (!clickEnabled)
            {
                return;
            }
            
            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }
    }
}
