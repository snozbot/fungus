// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;


namespace Fungus
{
    /// <summary>
    /// The block will execute when the player drags an object and successfully drops it on a target object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Completed",
                      "The block will execute when the player drags an object and successfully drops it on a target object.")]
    [AddComponentMenu("")]
    public class DragCompleted : EventHandler, ISerializationCallbackReceiver
    {   
        public class DragCompletedEvent
        {
            public Draggable2D DraggableObject;
            public DragCompletedEvent(Draggable2D draggableObject)
            {
                DraggableObject = draggableObject;
            }
        }
        [VariableProperty(typeof(GameObjectVariable))]

        [SerializeField] protected GameObjectVariable draggableRef;
        [VariableProperty(typeof(GameObjectVariable))]
        [SerializeField] protected GameObjectVariable targetRef;

       
        [Tooltip("Draggable object to listen for drag events on")]
        [HideInInspector]
        [SerializeField] protected Draggable2D draggableObject;
        
        [SerializeField] protected List<Draggable2D> draggableObjects;

        [Tooltip("Drag target object to listen for drag events on")]
        [HideInInspector]
        [SerializeField] protected Collider2D targetObject;
        [SerializeField] protected List<Collider2D> targetObjects;





        
        // There's no way to poll if an object is touching another object, so
        // we have to listen to the callbacks and track the touching state ourselves.
        protected bool overTarget = false;

        protected Collider2D targetCollider = null;

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragCompletedEvent>(OnDragCompletedEvent);
            eventDispatcher.AddListener<DragEntered.DragEnteredEvent>(OnDragEnteredEvent);
            eventDispatcher.AddListener<DragExited.DragExitedEvent>(OnDragExitedEvent);

            foreach(Draggable2D dragObj in draggableObjects)
            {
                dragObj.RegisterHandler(this);
            }
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<DragCompletedEvent>(OnDragCompletedEvent);
            eventDispatcher.RemoveListener<DragEntered.DragEnteredEvent>(OnDragEnteredEvent);
            eventDispatcher.RemoveListener<DragExited.DragExitedEvent>(OnDragExitedEvent);

            foreach(Draggable2D dragObj in draggableObjects)
            {
                dragObj.UnregisterHandler(this);
            }

            eventDispatcher = null;
        }

        void OnDragCompletedEvent(DragCompletedEvent evt)
        {
            OnDragCompleted(evt.DraggableObject);
        }

        void OnDragEnteredEvent(DragEntered.DragEnteredEvent evt)
        {
            OnDragEntered(evt.DraggableObject, evt.TargetCollider);
        }

        void OnDragExitedEvent(DragExited.DragExitedEvent evt)
        {
            OnDragExited(evt.DraggableObject, evt.TargetCollider);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //add any dragableobject already present to list for backwards compatability
            if (draggableObject != null)
            {
                if (!draggableObjects.Contains(draggableObject))
                {
                    draggableObjects.Add(draggableObject);
                }
            }

            if (targetObject != null)
            {
                if (!targetObjects.Contains(targetObject))
                {
                    targetObjects.Add(targetObject);
                }
            }
            draggableObject = null;
            targetObject = null;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {

        }

        #region Public members

        /// <summary>
        /// Gets the draggable object.
        /// </summary>
        public virtual List<Draggable2D> DraggableObjects { get { return draggableObjects; } }

        /// <summary>
        /// Returns true if the draggable object is over the drag target object.
        /// </summary>
        public virtual bool IsOverTarget()
        {
            return overTarget;
        }

        /// <summary>
        /// Called by the Draggable2D object when the it enters the drag target.
        /// </summary>
        public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
        {
            if (this.targetObjects != null && this.draggableObjects !=null &&
                this.draggableObjects.Contains(draggableObject) &&
                this.targetObjects.Contains(targetObject))
            {
                overTarget = true;
                targetCollider = targetObject;
            }
        }
        
        /// <summary>
        /// Called by the Draggable2D object when the it exits the drag target.
        /// </summary>
        public virtual void OnDragExited(Draggable2D draggableObject, Collider2D targetObject)
        {
            if (this.targetObjects != null && this.draggableObjects !=null &&
                this.draggableObjects.Contains(draggableObject) &&
                this.targetObjects.Contains(targetObject))
            {
                overTarget = false;
                targetCollider = null;

            }
        }
        
        /// <summary>
        /// Called by the Draggable2D object when the the drag ends over the drag target.
        /// </summary>
        public virtual void OnDragCompleted(Draggable2D draggableObject)
        {
            if (this.draggableObjects.Contains(draggableObject) &&
                overTarget)
            {
                // Assume that the player will have to do perform another drag and drop operation
                // to complete the drag again. This is necessary because we don't get an OnDragExited if the
                // draggable object is set to be inactive.
                if(draggableRef!=null)
                {
                    draggableRef.Value = draggableObject.gameObject;
                }
                if(targetRef!=null)
                {
                    targetRef.Value = targetCollider.gameObject;
                }

                overTarget = false;
                targetCollider = null;

                ExecuteBlock();
            }
        }


        public override string GetSummary()
        {
            string summary = "Draggable: ";
            if (this.draggableObjects != null && this.draggableObjects.Count != 0)
            {
                for (int i = 0; i < this.draggableObjects.Count; i++)
                {
                    if (draggableObjects[i] != null)
                    {
                        summary += draggableObjects[i].name + ",";
                    }   
                }
            }

            summary += "\nTarget: ";
            if (this.targetObjects != null && this.targetObjects.Count != 0)
            {
                for (int i = 0; i < this.targetObjects.Count; i++)
                {
                    if (targetObjects[i] != null)
                    {
                        summary += targetObjects[i].name + ",";
                    }   
                }
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