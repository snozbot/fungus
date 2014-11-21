using UnityEngine;
using System.Collections;

namespace Fungus 
{
	[EventHandlerInfo("Sprites",
	                  "Object Clicked",
	                  "The sequence will execute when the user clicks or taps on the clickable object.")]
	public class ObjectClicked : EventHandler
	{	
		public Clickable2D clickableObject;
		
		public virtual void OnObjectClicked(Clickable2D clickableObject)
		{
			if (clickableObject == this.clickableObject)
			{
				ExecuteSequence();
			}
		}
	}
}
