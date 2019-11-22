// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class Condition : Command
    {
        public static string GetOperatorDescription(CompareOperator compareOperator)
        {
            string summary = "";
            switch (compareOperator)
            {
            case CompareOperator.Equals:
                summary += "==";
                break;
            case CompareOperator.NotEquals:
                summary += "!=";
                break;
            case CompareOperator.LessThan:
                summary += "<";
                break;
            case CompareOperator.GreaterThan:
                summary += ">";
                break;
            case CompareOperator.LessThanOrEquals:
                summary += "<=";
                break;
            case CompareOperator.GreaterThanOrEquals:
                summary += ">=";
                break;
            }

            return summary;
        }

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
                System.Type previousCommandType = ParentBlock.GetPreviousActiveCommandType();
                var prevCmdIndent = ParentBlock.GetPreviousActiveCommandIndent();

                //handle our matching if or else if in the chain failing and moving to us,
                //  need to make sure it is the same indent level
                if (prevCmdIndent == IndentLevel && previousCommandType.IsSubclassOf(typeof(Condition)))
                {
                    // Else If behaves the same as an If command
                    EvaluateAndContinue();
                }
                else
                {
                    // Else If behaves mostly like an Else command, 
                    // but will also jump to a following Else command.

                    // Stop if this is the last command in the list
                    if (CommandIndex >= ParentBlock.CommandList.Count - 1)
                    {
                        StopParentBlock();
                        return;
                    }

                    // Find the next End command at the same indent level as this Else If command
                    int indent = indentLevel;
                    for (int i = CommandIndex + 1; i < ParentBlock.CommandList.Count; ++i)
                    {
                        var command = ParentBlock.CommandList[i];

                        if (command.IndentLevel == indent)
                        {
                            System.Type type = command.GetType();
                            if (type == typeof(End))
                            {
                                // Execute command immediately after the Else or End command
                                Continue(command.CommandIndex + 1);
                                return;
                            }
                        }
                    }

                    // No End command found
                    StopParentBlock();
                }
            }
        }

        public override bool OpenBlock()
        {
            return true;
        }

        #endregion

        protected virtual void EvaluateAndContinue()
        {
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
    }
}