using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Sprites",
	                  "Drag Started",
	                  "The sequence will execute when the player starts dragging an object.")]
	public class DragStarted : EventHandler
	{	
		public Draggable2D draggableObject;

		public virtual void OnDragStarted(Draggable2D draggableObject)
		{
			if (draggableObject == this.draggableObject)
			{
				ExecuteSequence();
			}
		}
	}
}
