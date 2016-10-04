// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.
    /// </summary>
    [CommandInfo("Flow", 
                 "While", 
                 "Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.")]
    [AddComponentMenu("")]
    public class While : If
    {
        #region Public members

        public override void OnEnter()
        {
            bool execute = true;
            if (variable != null)
            {
                execute = EvaluateCondition();
            }

            // Find next End statement at same indent level
            End endCommand = null;
            for (int i = CommandIndex + 1; i < ParentBlock.CommandList.Count; ++i)
            {
                End command = ParentBlock.CommandList[i] as End;
                
                if (command != null && 
                    command.IndentLevel == indentLevel)
                {
                    endCommand = command;
                    break;
                }
            }

            if (execute)
            {
                // Tell the following end command to loop back
                endCommand.Loop = true;
                Continue();
            }
            else
            {
                // Continue at next command after End
                Continue (endCommand.CommandIndex + 1);
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

        #endregion
    }    
}