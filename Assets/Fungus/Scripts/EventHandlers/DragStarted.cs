// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;

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
        [SerializeField] protected VariableReference draggableRef;
        [SerializeField] protected List<Draggable2D> draggableObjects;

        [HideInInspector]
        [SerializeField] protected Draggable2D draggableObject;


        protected EventDispatcher eventDispatcher;


        protected virtual void OnEnable()
        {
            eventDispatcher = FungusManager.Instance.EventDispatcher;

            eventDispatcher.AddListener<DragStartedEvent>(OnDragStartedEvent);
        }

        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        void OnValidate()
        {
            //add any dragableobject already present to list for backwards compatability
            if(draggableObject!=null){
                if(!draggableObjects.Contains(draggableObject)){
                    draggableObjects.Add(draggableObject);
                }
            }
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
            if (draggableObjects.Contains(draggableObject))
            {
                draggableRef.Set<GameObject>(draggableObject.gameObject);
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

            if (summary.Length == 0)
            {
                return "None";
            }

            return summary;            
        }

        #endregion
    }
}
