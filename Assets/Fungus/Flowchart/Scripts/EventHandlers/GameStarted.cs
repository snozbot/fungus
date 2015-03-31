using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Game Started",
	                  "The block will execute when the game starts playing.")]
	[AddComponentMenu("")]
	public class GameStarted : EventHandler
	{	
		protected virtual void Start()
		{
			ExecuteBlock();
		}
	}
}
