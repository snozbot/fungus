using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class SetBooleanCommand : FungusCommand 
	{
		public string key;

		public bool value;

		public override void OnExecute()
		{
			Variables.SetBoolean(key, value);
			ExecuteNextCommand();
		}
	}
	
}