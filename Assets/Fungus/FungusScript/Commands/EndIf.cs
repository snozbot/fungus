using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "EndIf", 
	             "Marks the end of an If statement block.", 
	             253, 253, 150)]
	public class EndIf : FungusCommand
	{
		public override void OnEnter()
		{
			Continue();
		}

		public override int GetPreIndent()
		{
			return -1;
		}
	}

}