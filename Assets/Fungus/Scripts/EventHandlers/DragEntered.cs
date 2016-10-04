// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the player is dragging an object which starts touching the target object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Entered",
                      "The block will execute when the player is dragging an object which starts touching the target object.")]
    [AddComponentMenu("")]
    public class DragEntered : EventHandler
    {   
        [Tooltip("Draggable object to listen for drag events on")]
        [SerializeField] protected Draggable2D draggableObject;

        [Tooltip("Drag target object to listen for drag events on")]
        [SerializeField] protected Collider2D targetObject;

        #region Public members

        /// <summary>
        /// Called by the Draggable2D object when the the drag enters the drag target.
        /// </summary>
        public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
        {
            if (draggableObject == this.draggableObject &&
                targetObject == this.targetObject)
            {
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            string summary = "";
            if (draggableObject != null)
            {
                summary += "\nDraggable: " + draggableObject.name;
            }
            if (targetObject != null)
            {
                summary += "\nTarget: " + targetObject.name;
            }
            
            if (summary.Length == 0)
            {
                return "None";
            }
            
            return summary;
        }

        #endregion
    }
}
