using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

	[CommandInfo("Flow", 
	             "Else If", 
	             "Marks the start of a command block to be executed when the preceding If statement is False and the test expression is true.")]
	[AddComponentMenu("")]
	public class ElseIf : If
	{

		public override void OnEnter()
		{
			System.Type previousCommandType = parentBlock.GetPreviousActiveCommandType();

			if (previousCommandType == typeof(If) ||
			    previousCommandType == typeof(ElseIf) )
			{
				// Else If behaves the same as an If command
				EvaluateAndContinue();
			}
			else
			{
				// Else If behaves mostly like an Else command, 
				// but will also jump to a following Else command.

				// Stop if this is the last command in the list
				if (commandIndex >= parentBlock.commandList.Count - 1)
				{
					Stop();
					return;
				}

				// Find the next End command at the same indent level as this Else If command
				int indent = indentLevel;
				for (int i = commandIndex + 1; i < parentBlock.commandList.Count; ++i)
				{
					Command command = parentBlock.commandList[i];

					if (command.indentLevel == indent)
					{
						System.Type type = command.GetType();
						if (type == typeof(End))
						{
							// Execute command immediately after the Else or End command
							Continue(command.commandIndex + 1);
							return;
						}
					}
				}

				// No End command found
				Stop();
			}
		}

		public override bool OpenBlock()
		{
			return true;
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