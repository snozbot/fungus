using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Object Enabled",
	                  "The sequence will execute when the Fungus Script game object is enabled.")]
	public class ObjectEnabled : EventHandler
	{	
		protected virtual void OnEnable()
		{
			ExecuteSequence();
		}
	}
}
