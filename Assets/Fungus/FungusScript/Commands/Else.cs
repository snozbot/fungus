using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "Else", 
	             "Marks the start of a sequence block to be executed when the preceding If statement is false.", 
	             253, 253, 150)]
	public class Else : FungusCommand
	{
		public override void OnEnter()
		{
			Continue();
		}

		public override int GetPreIndent()
		{
			return -1;
		}

		public override int GetPostIndent()
		{
			return 1;
		}
	}

}