using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Start Game",
	                  "The sequence will execute when the game starts playing.")]
	public class StartGame : EventHandler
	{	
		protected virtual void Start()
		{
			ExecuteSequence();
		}
	}
}
