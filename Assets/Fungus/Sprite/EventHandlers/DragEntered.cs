using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	[EventHandlerInfo("Sprites",
	                  "Drag Entered",
	                  "The sequence will execute when the player is dragging an object which starts touching the target object.")]
	public class DragEntered : EventHandler
	{	
		public Draggable2D draggableObject;
		public Collider2D targetObject;

		public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
		{
			if (draggableObject == this.draggableObject &&
			    targetObject == this.targetObject)
			{
				ExecuteSequence();
			}
		}
	}
}
