using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Jump", 
	             "Move execution to a specific Label command")]
	[AddComponentMenu("")]
	public class Jump : Command
	{
		[Tooltip("Label to jump to")]
		public Label targetLabel;

		public override void OnEnter()
		{
			if (targetLabel == null)
			{
				Continue();
				return;
			}

			foreach (Command command in parentBlock.commandList)
			{
				Label label = command as Label;
				if (label != null &&
				    label == targetLabel)
				{
					Continue(label.commandIndex + 1);
					break;
				}
			}
		}

		public override string GetSummary()
		{
			if (targetLabel == null)
			{
				return "Error: No label selected";
			}

			return targetLabel.key;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}