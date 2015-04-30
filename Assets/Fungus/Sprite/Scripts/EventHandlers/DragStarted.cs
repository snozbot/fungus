using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Sprite",
	                  "Drag Started",
	                  "The block will execute when the player starts dragging an object.")]
	[AddComponentMenu("")]
	public class DragStarted : EventHandler
	{	
		public Draggable2D draggableObject;

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
	}
}
