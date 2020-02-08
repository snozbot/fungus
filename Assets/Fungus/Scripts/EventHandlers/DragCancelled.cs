// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;


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
        [SerializeField] protected List<Draggable2D> draggableObjects;

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
            for (int i = 0; i < this.draggableObjects.Count; i++)
            {
                if (draggableObject == this.draggableObjects[i])
                {
                    ExecuteBlock();
                } 
                
            }
        }

        public override string GetSummary()
        {
            string summary = "Dragable: ";
            if (this.draggableObjects != null && this.draggableObjects.Count != 0)
            {
                for (int i = 0; i < this.draggableObjects.Count; i++)
                {
                    if (draggableObjects[i] != null)
                    {
                        summary += draggableObjects[i].name + ",";
                    }   
                }
                return summary;
            }
            else
            {
                return "None";
            }
            
        }

        #endregion
    }
}
