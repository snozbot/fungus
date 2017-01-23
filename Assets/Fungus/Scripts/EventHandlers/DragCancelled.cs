// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the player drags an object and releases it without dropping it on a target object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Cancelled",
                      "The block will execute when the player drags an object and releases it without dropping it on a target object.")]
    [AddComponentMenu("")]
    public class DragCancelled : EventHandler
    {   
        public class DragCancelledEvent
        {
            public Draggable2D DraggableObject;
            public DragCancelledEvent(Draggable2D draggableObject)
            {
                DraggableObject = draggableObject;
            }
        }

        [Tooltip("Draggable object to listen for drag events on")]
        [SerializeField] protected Draggable2D draggableObject;

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragCancelledEvent>(OnDragCancelledEvent);
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<DragCancelledEvent>(OnDragCancelledEvent);

            eventDispatcher = null;
        }

        protected virtual void OnDragCancelledEvent(DragCancelledEvent evt)
        {
            OnDragCancelled(evt.DraggableObject);
        }

        #region Public members

        public virtual void OnDragCancelled(Draggable2D draggableObject)
        {
            if (draggableObject == this.draggableObject)
            {
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            if (draggableObject != null)
            {
                return draggableObject.name;
            }
            
            return "None";
        }

        #endregion
    }
}
