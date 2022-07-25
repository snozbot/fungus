// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Base for all Conditional based Commands, Ifs, Loops, and so on.
    /// </summary>
    [AddComponentMenu("")]
    public abstract class Condition : Command
    {
        protected End endCommand;
     
        public override void OnEnter()
        {
            if (ParentBlock == null)
            {
                return;
            }

            //if looping we need the end command in order to work
            if(IsLooping && !EnsureRequiredEnd())
            {
                Debug.LogError(GetLocationIdentifier() + " is looping but has no matching End command");
                Continue();
                return;
            }

            if ( !HasNeededProperties() )
            {
                Debug.LogError(GetLocationIdentifier() + " cannot run due to missing required properties");
                Continue();
                return;
            }

            //Ensuring we arrived at this elif honestly, not incorrectly due to fall through from a previous command
            if (this.IsElseIf && !DoesPassElifSanityCheck())
            {
                //elif is being asked to run but didn't come from a previously failing if or elif, this isn't allowed
                MoveToEnd();
                return;
            }

            EvaluateAndContinue();
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


        /// <summary>
        /// Moves execution to the closing End of the current command, attempts to locate end if not
        /// already known and if no closing End exists.
        /// </summary>
        public virtual void MoveToEnd()
        {
            if(endCommand == null)
            {
                endCommand = FindOurEndCommand();
            }

            if (endCommand != null)
            {
                // Continue at next command after End
                // and make the end non looping incase it gets run via index etc.
                endCommand.Loop = false;
                Continue(endCommand.CommandIndex + 1);
            }
            else
            {
                //nowhere to go, so we assume the block wants to stop but is missing and end, this
                //  is also ensures back compat
                Debug.LogWarning("Condition wants to move to end but no End command found, stopping block. " + GetLocationIdentifier());
                StopParentBlock();
            }
        }

               
        protected End FindOurEndCommand()
        {
            return FindMatchingEndCommand(this);
        }

        /// <summary>
        /// Helper to find the paired End Command for the given command.
        /// </summary>
        /// <param name="startCommand"></param>
        /// <returns>Mathcing End Command or null if not found</returns>
        public static End FindMatchingEndCommand(Command startCommand)
        {
            if (startCommand.ParentBlock == null)
                return null;

            int indent = startCommand.IndentLevel;
            for (int i = startCommand.CommandIndex + 1; i < startCommand.ParentBlock.CommandList.Count; ++i)
            {
                var command = startCommand.ParentBlock.CommandList[i];

                if (command.IndentLevel == indent)
                {
                    if (command is End)
                    {
                        return command as End;
                    }
                }
                else if (command.IndentLevel < indent)
                {
                    //managed to be less indent than the inner but not find and end, this shouldn't occur
                    // but may be user error or bad data, makes sense for completeness here
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// Helper for child classes that require an End command to function. For IsLooping commands
        /// this also configures the loopback within the End command.
        /// </summary>
        /// <returns></returns>
        protected virtual bool EnsureRequiredEnd()
        {
            if (endCommand == null)
            {
                endCommand = FindOurEndCommand();

                if (endCommand == null)
                {
                    Debug.LogError( GetLocationIdentifier() + "', could not find closing End command and thus cannot loop.");
                    //StopParentBlock();
                    return false;
                }
            }

            if (IsLooping)
            {
                // Tell the following end command to loop back
                endCommand.Loop = true;
                endCommand.LoopBackIndex = CommandIndex;
            }
            return true;
        }

        /// <summary>
        /// Called by OnEnter when the condition is needed to evaluate and continue execution.
        /// Means child classes do not have to deal with erronuous execution conditions, like fall through.
        /// </summary>
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

        /// <summary>
        /// Called when the condition is run and EvaluateCondition returns true 
        /// </summary>
        protected virtual void OnTrue()
        {
            Continue();
        }

        /// <summary>
        /// Called when the condition is run and EvaluateCondition returns false 
        /// </summary>
        protected virtual void OnFalse()
        {
            //looping constructs only care about the end
            if(IsLooping)
            {
                MoveToEnd();
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

        /// <summary>
        /// Sits in the if within EvaluateAndContinue, if returns true, OnTrue will run, if false, OnFalse will run.
        /// </summary>
        protected abstract bool EvaluateCondition();

        /// <summary>
        /// Child classes are required to report if it is possible for them to be evaulated.
        /// </summary>
        protected virtual bool HasNeededProperties() { return true; }

        /// <summary>
        /// Declare if the child class is implementing an 'else if' command, which requires some special handling
        /// </summary>
        protected virtual bool IsElseIf { get { return false; } }

        /// <summary>
        /// Called before EvaluateCondition, allowing for child classes to gather required data
        /// </summary>
        protected virtual void PreEvaluate() { }

        /// <summary>
        /// Ensure that this condition didn't come from a non matching if/elif.
        /// </summary>
        /// <returns></returns>
        protected virtual bool DoesPassElifSanityCheck()
        {
            System.Type previousCommandType = ParentBlock.GetPreviousActiveCommandType();
            var prevCmdIndent = ParentBlock.GetPreviousActiveCommandIndent();
            var prevCmd = ParentBlock.GetPreviousActiveCommand();

            //handle our matching if or else if in the chain failing and moving to us,
            //  need to make sure it is the same indent level
            if (prevCmd == null ||
                prevCmdIndent != IndentLevel ||
                !previousCommandType.IsSubclassOf(typeof(Condition)) ||
                (prevCmd as Condition).IsLooping)
            {
                return false;
            }

            return true;
        }
    }
}