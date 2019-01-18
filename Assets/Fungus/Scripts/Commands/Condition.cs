// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class Condition : Command
    {
        protected End endCommand;
     
        #region Public members

        public override void OnEnter()
        {
            if (ParentBlock == null)
            {
                return;
            }

            if( !HasNeededProperties() )
            {
                Continue();
                return;
            }

            if( !this.IsElseIf )
            {
                EvaluateAndContinue();
            }
            else
            {
                ElIfEvalAndContinue();
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
        
        public virtual bool IsLooping { get { return false; } }


        public virtual void MoveToEnd()
        {
            if (endCommand != null)
            {
                // Continue at next command after End
                //but make the end non looping incase it gets run via index etc.
                endCommand.Loop = false;
                Continue(endCommand.CommandIndex + 1);
            }
            else
            {
                StopParentBlock();
            }
        }

        #endregion

        protected virtual void ElIfEvalAndContinue()
        {
            // Else If behaves mostly like an Else command, 
            // but will also jump to a following Else command.

            System.Type previousCommandType = ParentBlock.GetPreviousActiveCommandType();
            var prevCmdIndent = ParentBlock.GetPreviousActiveCommandIndent();

            //handle our matching if or else if in the chain failing and moving to us,
            //  need to make sure it is the same indent level
            if (prevCmdIndent == IndentLevel && previousCommandType.IsSubclassOf(typeof(Condition)))
            {
                // Else If behaves the same as an If command
                EvaluateAndContinue();
            }
            else //we didn't come from a previous if or elif so get out of here
            {

                // Stop if this is the last command in the list
                if (CommandIndex >= ParentBlock.CommandList.Count - 1)
                {
                    StopParentBlock();
                    return;
                }

                // Find the next End command at the same indent level as this Else If command
                var end = FindOurEndCommand();
                int endIndex = end != null ? end.CommandIndex : -1;
                if (endIndex != -1)
                {
                    // Execute command immediately after the Else or End command
                    Continue(endIndex + 1);
                }
                else
                {
                    // No End command found
                    StopParentBlock();
                }
            }
        }

        protected End FindOurEndCommand()
        {
            int indent = indentLevel;
            for (int i = CommandIndex + 1; i < ParentBlock.CommandList.Count; ++i)
            {
                var command = ParentBlock.CommandList[i];

                if (command.IndentLevel == indent)
                {
                    System.Type type = command.GetType();
                    if (type == typeof(End))
                    {
                        return command as End;
                    }
                }
                else if (command.IndentLevel < indent)
                {
                    //managed to be less indent than the inner but not find and end, this shouldn't occur
                    // but makes sense for completeness here
                    return null;
                }
            }

            return null;
        }

        protected virtual bool HasRequiredEnd(bool loopback)
        {
            if (endCommand == null)
            {
                endCommand = FindOurEndCommand();

                if (endCommand == null)
                {
                    Debug.LogError(GetType().Name + " command in, '" + ParentBlock.BlockName + "' on '" + ParentBlock.GetFlowchart().gameObject.name +
                      "', could not find closing End command and thus cannot loop.");
                    //StopParentBlock();
                    return false;
                }
            }

            if (loopback)
            {
                // Tell the following end command to loop back
                endCommand.Loop = true;
                endCommand.LoopBackIndex = CommandIndex;
            }
            return true;
        }

        protected virtual void EvaluateAndContinue()
        {
            PreEvaluate();

            if (EvaluateCondition())
            {
                OnTrue();
            }
            else
            {
                OnFalse();
            }
        }

        protected virtual void OnTrue()
        {
            Continue();
        }

        protected virtual void OnFalse()
        {
            // Last command in block
            if (CommandIndex >= ParentBlock.CommandList.Count)
            {
                StopParentBlock();
                return;
            }

            // Find the next Else, ElseIf or End command at the same indent level as this If command
            for (int i = CommandIndex + 1; i < ParentBlock.CommandList.Count; ++i)
            {
                Command nextCommand = ParentBlock.CommandList[i];

                if (nextCommand == null)
                {
                    continue;
                }

                // Find next command at same indent level as this If command
                // Skip disabled commands, comments & labels
                if (!((Command)nextCommand).enabled || 
                    nextCommand.GetType() == typeof(Comment) ||
                    nextCommand.GetType() == typeof(Label) ||
                    nextCommand.IndentLevel != indentLevel)
                {
                    continue;
                }

                System.Type type = nextCommand.GetType();
                if (type == typeof(Else) ||
                    type == typeof(End))
                {
                    if (i >= ParentBlock.CommandList.Count - 1)
                    {
                        // Last command in Block, so stop
                        StopParentBlock();
                    }
                    else
                    {
                        // Execute command immediately after the Else or End command
                        Continue(nextCommand.CommandIndex + 1);
                        return;
                    }
                }
                else if (type.IsSubclassOf(typeof(Condition)) && (nextCommand as Condition).IsElseIf)
                {
                    // Execute the Else If command
                    Continue(i);

                    return;
                }
            }

            // No matching End command found, so just stop the block
            StopParentBlock();
        }

        protected abstract bool EvaluateCondition();

        protected abstract bool HasNeededProperties();

        protected virtual bool IsElseIf { get { return false; } }

        protected virtual void PreEvaluate() { }
    }
}