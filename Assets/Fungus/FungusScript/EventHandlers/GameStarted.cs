using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Game Started",
	                  "The sequence will execute when the game starts playing.")]
	public class GameStarted : EventHandler
	{	
		protected virtual void Start()
		{
			ExecuteSequence();
		}
	}
}
