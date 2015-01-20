using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Jump", 
	             "Move execution to a specific Label command")]
	[AddComponentMenu("")]
	public class Jump : Command
	{
		public string labelName;

		public override void OnEnter()
		{
			if (labelName.Length == 0)
			{
				Continue();
				return;
			}

			foreach (Command command in parentSequence.commandList)
			{
				Label label = command as Label;
				if (label != null &&
					label.labelName == labelName)
				{
					Continue(label.commandIndex + 1);
					break;
				}
			}
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}