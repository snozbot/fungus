using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Note: The End If command is deprecated, use the End command instead.
	[CommandInfo("Scripting", 
	             "End", 
	             "Marks the end of a conditional block.", -1)]
	[AddComponentMenu("")]
	public class EndIf : Command
	{
		public override void OnEnter()
		{
			Continue();
		}

		public override bool CloseBlock()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}