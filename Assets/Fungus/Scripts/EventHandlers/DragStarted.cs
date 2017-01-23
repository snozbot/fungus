// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the player starts dragging an object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Started",
                      "The block will execute when the player starts dragging an object.")]
    [AddComponentMenu("")]
    public class DragStarted : EventHandler
    {   
        public class DragStartedEvent
        {
            public Draggable2D DraggableObject;
            public DragStartedEvent(Draggable2D draggableObject)
            {
                DraggableObject = draggableObject;
            }
        }

        [SerializeField] protected Draggable2D draggableObject;

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragStartedEvent>(OnDragStartedEvent);
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<DragStartedEvent>(OnDragStartedEvent);

            eventDispatcher = null;
        }

        void OnDragStartedEvent(DragStartedEvent evt)
        {
            OnDragStarted(evt.DraggableObject);
        }

        #region Public members

        /// <summary>
        /// Called by the Draggable2D object when the drag starts.
        /// </summary>
        public virtual void OnDragStarted(Draggable2D draggableObject)
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
