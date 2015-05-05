using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Sprite",
	                  "Drag Cancelled",
	                  "The block will execute when the player drags an object and releases it without dropping it on a target object.")]
	[AddComponentMenu("")]
	public class DragCancelled : EventHandler
	{	
		[Tooltip("Draggable object to listen for drag events on")]
		public Draggable2D draggableObject;
		
		public virtual void OnDragCancelled(Draggable2D draggableObject)
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
