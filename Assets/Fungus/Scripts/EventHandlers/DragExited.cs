// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;


namespace Fungus
{
    /// <summary>
    /// The block will execute when the player is dragging an object which stops touching the target object.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Exited",
                      "The block will execute when the player is dragging an object which stops touching the target object.")]
    [AddComponentMenu("")]
    public class DragExited : EventHandler, ISerializationCallbackReceiver
    {   
        public class DragExitedEvent
        {
            public Draggable2D DraggableObject;
            public Collider2D TargetCollider;
            public DragExitedEvent(Draggable2D draggableObject, Collider2D targetCollider)
            {
                DraggableObject = draggableObject;
                TargetCollider = targetCollider;
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

       

        protected EventDispatcher eventDispatcher;

        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragExitedEvent>(OnDragEnteredEvent);
        }

        protected virtual void OnDisable()
        {
            eventDispatcher.RemoveListener<DragExitedEvent>(OnDragEnteredEvent);

            eventDispatcher = null;
        }

        void OnDragEnteredEvent(DragExitedEvent evt)
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
        /// Called by the Draggable2D object when the drag exits from the targetObject.
        /// </summary>
        public virtual void OnDragExited(Draggable2D draggableObject, Collider2D targetObject)
        {
             if (this.targetObjects != null && this.draggableObjects !=null &&
                this.draggableObjects.Contains(draggableObject) &&
                this.targetObjects.Contains(targetObject))
            {
                if(draggableRef!=null)
                {
                    draggableRef.Value = draggableObject.gameObject;
                }
                if(targetRef!=null)
                {
                    targetRef.Value = targetObject.gameObject;
                }
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
