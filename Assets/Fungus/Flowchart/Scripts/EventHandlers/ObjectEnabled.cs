using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Object Enabled",
	                  "The block will execute when the Flowchart game object is enabled.")]
	[AddComponentMenu("")]
	public class ObjectEnabled : EventHandler
	{	
		protected virtual void OnEnable()
		{
			ExecuteBlock();
		}
	}
}
