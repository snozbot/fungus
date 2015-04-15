using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "While", 
	             "Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.")]
	[AddComponentMenu("")]
	public class While : If
	{
		public override void OnEnter()
		{
			bool execute = true;
			if (variable != null)
			{
				execute = EvaluateCondition();
			}

			// Find next End statement at same indent level
			End endCommand = null;
			for (int i = commandIndex + 1; i < parentBlock.commandList.Count; ++i)
			{
				End command = parentBlock.commandList[i] as End;
				
				if (command != null && 
				    command.indentLevel == indentLevel)
				{
					endCommand = command;
				}
			}

			if (execute)
			{
				// Tell the following end command to loop back
				endCommand.loop = true;
				Continue();
			}
			else
			{
				// Continue at next command after End
				Continue (endCommand.commandIndex + 1);
			}
		}

		public override bool OpenBlock()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}		
	}
	
}