/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Else", 
                 "Marks the start of a command block to be executed when the preceding If statement is False.")]
    [AddComponentMenu("")]
    public class Else : Command
    {
        public override void OnEnter()
        {
            if (parentBlock == null)
            {
                return;
            }

            // Stop if this is the last command in the list
            if (commandIndex >= parentBlock.commandList.Count - 1)
            {
                StopParentBlock();
                return;
            }

            // Find the next End command at the same indent level as this Else command
            int indent = indentLevel;
            for (int i = commandIndex + 1; i < parentBlock.commandList.Count; ++i)
            {
                Command command = parentBlock.commandList[i];
                
                if (command.indentLevel == indent)
                {
                    System.Type type = command.GetType();
                    if (type == typeof(End))
                    {
                        // Execute command immediately after the EndIf command
                        Continue(command.commandIndex + 1);
                        return;
                    }
                }
            }
            
            // No End command found
            StopParentBlock();
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