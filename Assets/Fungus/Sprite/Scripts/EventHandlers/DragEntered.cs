using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	[EventHandlerInfo("Sprite",
	                  "Drag Entered",
	                  "The block will execute when the player is dragging an object which starts touching the target object.")]
	[AddComponentMenu("")]
	public class DragEntered : EventHandler
	{	
		[Tooltip("Draggable object to listen for drag events on")]
		public Draggable2D draggableObject;

		[Tooltip("Drag target object to listen for drag events on")]
		public Collider2D targetObject;

		public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
		{
			if (draggableObject == this.draggableObject &&
			    targetObject == this.targetObject)
			{
				ExecuteBlock();
			}
		}

		public override string GetSummary()
		{
			string summary = "";
			if (draggableObject != null)
			{
				summary += "\nDraggable: " + draggableObject.name;
			}
			if (targetObject != null)
			{
				summary += "\nTarget: " + targetObject.name;
			}
			
			if (summary.Length == 0)
			{
				return "None";
			}
			
			return summary;
		}
	}
}
