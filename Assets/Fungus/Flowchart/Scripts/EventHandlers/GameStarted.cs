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
