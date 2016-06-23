/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Sprite",
	                  "Drag Completed",
	                  "The block will execute when the player drags an object and successfully drops it on a target object.")]
	[AddComponentMenu("")]
	public class DragCompleted : EventHandler
	{	
		[Tooltip("Draggable object to listen for drag events on")]
		public Draggable2D draggableObject;

		[Tooltip("Drag target object to listen for drag events on")]
		public Collider2D targetObject;
		
		// There's no way to poll if an object is touching another object, so
		// we have to listen to the callbacks and track the touching state ourselves.
		bool overTarget = false;
		
		public virtual bool IsOverTarget()
		{
			return overTarget;
		}
		
		public virtual void OnDragEntered(Draggable2D draggableObject, Collider2D targetObject)
		{
			if (this.targetObject != null &&
			    draggableObject == this.draggableObject &&
			    targetObject == this.targetObject)
			{
				overTarget = true;
			}
		}
		
		public virtual void OnDragExited(Draggable2D draggableObject, Collider2D targetObject)
		{
			if (this.targetObject != null &&
			    draggableObject == this.draggableObject &&
			    targetObject == this.targetObject)
			{
				overTarget = false;
			}
		}
		
		public virtual void OnDragCompleted(Draggable2D draggableObject)
		{
			if (draggableObject == this.draggableObject &&
			    overTarget)
			{
				// Assume that the player will have to do perform another drag and drop operation
				// to complete the drag again. This is necessary because we don't get an OnDragExited if the
				// draggable object is set to be inactive.
				overTarget = false;
				
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