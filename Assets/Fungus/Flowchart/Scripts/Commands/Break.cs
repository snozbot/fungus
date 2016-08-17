/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Break", 
                 "Force a loop to terminate immediately.")]
    [AddComponentMenu("")]
    public class Break : Command
    {
        public override void OnEnter()
        {
            // Find index of previous while command
            int whileIndex = -1;
            int whileIndentLevel = -1;
            for (int i = commandIndex - 1; i >=0; --i)
            {
                While whileCommand = parentBlock.commandList[i] as While;
                if (whileCommand != null)
                {
                    whileIndex = i;
                    whileIndentLevel = whileCommand.indentLevel;
                    break;
                }
            }

            if (whileIndex == -1)
            {
                // No enclosing While command found, just continue
                Continue();
                return;
            }

            // Find matching End statement at same indent level as While
            for (int i = whileIndex + 1; i < parentBlock.commandList.Count; ++i)
            {
                End endCommand = parentBlock.commandList[i] as End;
                
                if (endCommand != null && 
                    endCommand.indentLevel == whileIndentLevel)
                {
                    // Sanity check that break command is actually between the While and End commands
                    if (commandIndex > whileIndex && commandIndex < endCommand.commandIndex)
                    {
                        // Continue at next command after End
                        Continue (endCommand.commandIndex + 1);
                        return;
                    }
                    else
                    {
                        break;
                    }
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