// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace Fungus
{
	// Dummy component for testing iterating over c# collections from Lua
	public class DummyCollection : MonoBehaviour 
	{
		public List<string> stringList = new List<string>();

		public List<Sprite> spriteList = new List<Sprite>();

		void Awake()
		{
			// Register this type with MoonSharp
			UserData.RegisterType(GetType());
		}
	}

}