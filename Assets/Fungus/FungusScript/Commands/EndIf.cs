using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "End If", 
	             "Marks the end of an If statement block.")]
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

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}