using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("Scripting",
	                  "Enable Object",
	                  "The sequence will execute when the owner game object is enabled.")]
	public class EnableObject : EventHandler
	{	
		protected virtual void OnEnable()
		{
			ExecuteSequence();
		}
	}
}
