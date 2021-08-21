// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the player is dragging an object which starts touching the target object.
    ///
    /// ExecuteAlways used to get the Compatibility that we need, use of ISerializationCallbackReceiver is error prone
    /// when used on Unity controlled objects as it runs on threads other than main thread.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Drag Entered",
                      "The block will execute when the player is dragging an object which starts touching the target object.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class DragEntered : EventHandler, ISerializationCallbackReceiver
    {
        public class DragEnteredEvent
        {
            public Draggable2D DraggableObject;
            public Collider2D TargetCollider;

            public DragEnteredEvent(Draggable2D draggableObject, Collider2D targetCollider)
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
            if (Application.isPlaying)
            {
                eventDispatcher = FungusManager.Instance.EventDispatcher;

                eventDispatcher.AddListener<DragEnteredEvent>(OnDragEnteredEvent);
            }
        }

        protected virtual void OnDisable()
        {
            if (Application.isPlaying)
            {
                eventDispatcher.RemoveListener<DragEnteredEvent>(OnDragEnteredEvent);

                eventDispatcher = null;
            }
        }

        private void OnDragEnteredEvent(DragEnteredEvent evt)
        {
            OnDragEntered(evt.DraggableObject, evt.TargetCollider);
        }

        #region Compatibility

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }

        private void Awake()
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

        #endregion Compatibility

        #region Public members

        /// <summary>
        /// Called by the Draggable2D object when the the drag enters the drag target.
        /// </summary>
        public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
        {
            if (draggableObject.BeingDragged &&
                this.targetObjects != null && this.draggableObjects != null &&
                this.draggableObjects.Contains(draggableObject) &&
                this.targetObjects.Contains(targetObject))
            {
                if (draggableRef != null)
                {
                    draggableRef.Value = draggableObject.gameObject;
                }
                if (targetRef != null)
                {
                    targetRef.Value = targetObject.gameObject;
                }
                ExecuteBlock();
            }
        }

        public override string GetSummary()
        {
            if (draggableObjects.Count(x => x != null) == 0)
            {
                return "Error: no draggable objects assigned.";
            }
            if (targetObjects.Count(x => x != null) == 0)
            {
                return "Error: no target objects assigned.";
            }

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

            return summary;
        }

        #endregion Public members
    }
}