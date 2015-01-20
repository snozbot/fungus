using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Label", 
	             "Marks a position in the command list for execution to jump to.")]
	[AddComponentMenu("")]
	public class Label : Command
	{
		public string labelName = "";

		public override void OnEnter()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return labelName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}