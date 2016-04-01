using UnityEngine;
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