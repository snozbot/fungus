using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Sprites",
	                  "Drag Cancelled",
	                  "The sequence will execute when the player drags an object and releases it without dropping it on a target object.")]
	public class DragCancelled : EventHandler
	{	
		public Draggable2D draggableObject;
		
		public virtual void OnDragCancelled(Draggable2D draggableObject)
		{
			if (draggableObject == this.draggableObject)
			{
				ExecuteSequence();
			}
		}
	}

}
