// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnMouse* message for the monobehaviour is received
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Mouse",
                      "The block will execute when the desired OnMouse* message for the monobehaviour is received")]
    [AddComponentMenu("")]
    public class Mouse : EventHandler
    {

        [System.Flags]
        public enum MouseMessageFlags
        {
            OnMouseDown     = 1 << 0,
            OnMouseDrag     = 1 << 1,
            OnMouseEnter    = 1 << 2,
            OnMouseExit     = 1 << 3,
            OnMouseOver     = 1 << 4,
            OnMouseUp       = 1 << 5,
            OnMouseUpAsButton = 1 << 6,
        }

        [Tooltip("Which of the Mouse messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected MouseMessageFlags FireOn = MouseMessageFlags.OnMouseUpAsButton;

        private void OnMouseDown()
        {
            HandleTriggering(MouseMessageFlags.OnMouseDown);
        }

        private void OnMouseDrag()
        {
            HandleTriggering(MouseMessageFlags.OnMouseDrag);
        }

        private void OnMouseEnter()
        {
            HandleTriggering(MouseMessageFlags.OnMouseEnter);
        }

        private void OnMouseExit()
        {
            HandleTriggering(MouseMessageFlags.OnMouseExit);
        }

        private void OnMouseOver()
        {
            HandleTriggering(MouseMessageFlags.OnMouseOver);
        }

        private void OnMouseUp()
        {
            HandleTriggering(MouseMessageFlags.OnMouseUp);
        }

        private void OnMouseUpAsButton()
        {
            HandleTriggering(MouseMessageFlags.OnMouseUpAsButton);
        }

        private void HandleTriggering(MouseMessageFlags from)
        {
            if((from & FireOn) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}
