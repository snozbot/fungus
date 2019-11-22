// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus
{
    /// <summary>
    /// Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
    /// The Game Object must have a Collider or Collider2D component attached.
    /// Use in conjunction with the ObjectClicked Flowchart event handler.
    /// </summary>
    public class Clickable2D : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Tooltip("Is object clicking enabled")]
        [SerializeField] protected bool clickEnabled = true;

        [Tooltip("Mouse texture to use when hovering mouse over object")]
        [SerializeField] protected Texture2D hoverCursor;

        [Tooltip("Use the UI Event System to check for clicks. Clicks that hit an overlapping UI object will be ignored. Camera must have a PhysicsRaycaster component, or a Physics2DRaycaster for 2D colliders.")]
        [SerializeField] protected bool useEventSystem;

        protected virtual void ChangeCursor(Texture2D cursorTexture)
        {
            if (!clickEnabled)
            {
                return;
            }

            Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
        }

        protected virtual void DoPointerClick()
        {
            if (!clickEnabled)
            {
                return;
            }

            var eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.Raise(new ObjectClicked.ObjectClickedEvent(this));
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

        #region Public members

        /// <summary>
        /// Is object clicking enabled.
        /// </summary>
        public bool ClickEnabled { set { clickEnabled = value; } }

        #endregion

        #region IPointerClickHandler implementation

        public void OnPointerClick(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerClick();
            }
        }

        #endregion

        #region IPointerEnterHandler implementation

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerEnter();
            }
        }

        #endregion

        #region IPointerExitHandler implementation

        public void OnPointerExit(PointerEventData eventData)
        {
            if (useEventSystem)
            {
                DoPointerExit();
            }
        }

        #endregion
    }
}
