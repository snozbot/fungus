using UnityEngine;
using System.Collections;

namespace Fungus 
{
	[EventHandlerInfo("Sprite",
	                  "Object Clicked",
	                  "The block will execute when the user clicks or taps on the clickable object.")]
	[AddComponentMenu("")]
	public class ObjectClicked : EventHandler
	{	
		[Tooltip("Object that the user can click or tap on")]
		public Clickable2D clickableObject;
		
		public virtual void OnObjectClicked(Clickable2D clickableObject)
		{
			if (clickableObject == this.clickableObject)
			{
				ExecuteBlock();
			}
		}

		public override string GetSummary()
		{
			if (clickableObject != null)
			{
				return clickableObject.name;
			}

			return "None";
		}
	}
}
