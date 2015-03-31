using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Break", 
	             "Force a loop to terminate immediately.")]
	[AddComponentMenu("")]
	public class Break : Command
	{
		public override void OnEnter()
		{
			// Find next End statement at -1 relative indent level
			for (int i = commandIndex + 1; i < parentBlock.commandList.Count; ++i)
			{
				End endCommand = parentBlock.commandList[i] as End;
				
				if (endCommand != null && 
				    endCommand.indentLevel == indentLevel - 1)
				{
					// Continue at next command after End
					Continue (endCommand.commandIndex + 1);
					return;
				}
			}

			// No matching End command found so just continue
			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}		
	}
	
}