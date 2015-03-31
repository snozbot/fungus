using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	[EventHandlerInfo("Sprites",
	                  "Drag Exited",
	                  "The block will execute when the player is dragging an object which stops touching the target object.")]
	[AddComponentMenu("")]
	public class DragExited : EventHandler
	{	
		public Draggable2D draggableObject;
		public Collider2D targetObject;

		public virtual void OnDragExited(Draggable2D draggableObject, Collider2D targetObject)
		{
			if (draggableObject == this.draggableObject &&
			    targetObject == this.targetObject)
			{
				ExecuteBlock();
			}
		}		
	}
}
