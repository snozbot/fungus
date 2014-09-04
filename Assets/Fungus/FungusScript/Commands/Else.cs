using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "Else", 
	             "Marks the start of a sequence block to be executed when the preceding If statement is false.")]
	public class Else : FungusCommand
	{
		public override void OnEnter()
		{
			// Find the next EndIf command at the same indent level as this Else command
			bool foundThisCommand = false;
			int indent = indentLevel;
			foreach(FungusCommand command in parentSequence.commandList)
			{
				if (foundThisCommand &&
				    command.indentLevel == indent)
				{
					System.Type type = command.GetType();
					if (type == typeof(EndIf))
					{
						// Execute command immediately after the EndIf command
						Continue(command);
						return;
					}
				}
				else if (command == this)
				{
					foundThisCommand = true;
				}
			}

			// No matching EndIf command found, so just stop the sequence
			Stop();
		}
		
		public override int GetPreIndent()
		{
			return -1;
		}

		public override int GetPostIndent()
		{
			return 1;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}