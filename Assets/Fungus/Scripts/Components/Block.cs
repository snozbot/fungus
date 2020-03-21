// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Execution state of a Block.
    /// </summary>
    public enum ExecutionState
    {
        /// <summary> No command executing </summary>
        Idle,       
        /// <summary> Executing a command </summary>
        Executing,
    }

    /// <summary>
    /// A container for a sequence of Fungus comands.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Flowchart))]
    [AddComponentMenu("")]
    public class Block : Node
    {
        [SerializeField] protected int itemId = -1; // Invalid flowchart item id

        [FormerlySerializedAs("sequenceName")]
        [Tooltip("The name of the block node as displayed in the Flowchart window")]
        [SerializeField] protected string blockName = "New Block";

        [TextArea(2, 5)]
        [Tooltip("Description text to display under the block node")]
        [SerializeField] protected string description = "";

        [Tooltip("An optional Event Handler which can execute the block when an event occurs")]
        [SerializeField] protected EventHandler eventHandler;

        [SerializeField] protected List<Command> commandList = new List<Command>();

        protected ExecutionState executionState;

        protected Command activeCommand;

        protected Action lastOnCompleteAction;

        /// <summary>
        // Index of last command executed before the current one.
        // -1 indicates no previous command.
        /// </summary>
        protected int previousActiveCommandIndex = -1;

        public int PreviousActiveCommandIndex { get { return previousActiveCommandIndex; } }

        protected int jumpToCommandIndex = -1;

        protected int executionCount;

        protected bool executionInfoSet = false;

        /// <summary>
        /// If set, flowchart will not auto select when it is next executed, used by eventhandlers.
        /// Only effects the editor.
        /// </summary>
        public bool SuppressNextAutoSelection { get; set; }

        [SerializeField] bool suppressAllAutoSelections = false;
        

        protected virtual void Awake()
        {
            SetExecutionInfo();
        }

        /// <summary>
        /// Populate the command metadata used to control execution.
        /// </summary>
        protected virtual void SetExecutionInfo()
        {
            // Give each child command a reference back to its parent block
            // and tell each command its index in the list.
            int index = 0;
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                if (command == null)
                {
                    continue;
                }
                command.ParentBlock = this;
                command.CommandIndex = index++;
            }

            // Ensure all commands are at their correct indent level
            // This should have already happened in the editor, but may be necessary
            // if commands are added to the Block at runtime.
            UpdateIndentLevels();

            executionInfoSet = true;
        }

#if UNITY_EDITOR
        // The user can modify the command list order while playing in the editor,
        // so we keep the command indices updated every frame. There's no need to
        // do this in player builds so we compile this bit out for those builds.
        protected virtual void Update()
        {
            int index = 0;
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                if (command == null)// Null entry will be deleted automatically later
                
                {
                    continue;
                }
                command.CommandIndex = index++;
            }
        }

#endif
        //editor only state for speeding up flowchart window drawing
        public bool IsSelected { get; set; }    //local cache of selectedness
        public enum FilteredState { Full, Partial, None}
        public FilteredState FilterState { get; set; }    //local cache of filteredness
        public bool IsControlSelected { get; set; } //local cache of being part of the control exclusion group

        #region Public members

        /// <summary>
        /// The execution state of the Block.
        /// </summary>
        public virtual ExecutionState State { get { return executionState; } }

        /// <summary>
        /// Unique identifier for the Block.
        /// </summary>
        public virtual int ItemId { get { return itemId; } set { itemId = value; } }

        /// <summary>
        /// The name of the block node as displayed in the Flowchart window.
        /// </summary>
        public virtual string BlockName { get { return blockName; } set { blockName = value; } }

        /// <summary>
        /// Description text to display under the block node
        /// </summary>
        public virtual string Description { get { return description; } }

        /// <summary>
        /// An optional Event Handler which can execute the block when an event occurs.
        /// Note: Using the concrete class instead of the interface here because of weird editor behaviour.
        /// </summary>
        public virtual EventHandler _EventHandler { get { return eventHandler; } set { eventHandler = value; } }

        /// <summary>
        /// The currently executing command.
        /// </summary>
        public virtual Command ActiveCommand { get { return activeCommand; } }

        /// <summary>
        /// Timer for fading Block execution icon.
        /// </summary>
        public virtual float ExecutingIconTimer { get; set; }

        /// <summary>
        /// The list of commands in the sequence.
        /// </summary>
        public virtual List<Command> CommandList { get { return commandList; } }

        /// <summary>
        /// Controls the next command to execute in the block execution coroutine.
        /// </summary>
        public virtual int JumpToCommandIndex { set { jumpToCommandIndex = value; } }

        /// <summary>
        /// Returns the parent Flowchart for this Block.
        /// </summary>
        public virtual Flowchart GetFlowchart()
        {
            return GetComponent<Flowchart>();
        }

        /// <summary>
        /// Returns true if the Block is executing a command.
        /// </summary>
        public virtual bool IsExecuting()
        {
            return (executionState == ExecutionState.Executing);
        }

        /// <summary>
        /// Returns the number of times this Block has executed.
        /// </summary>
        public virtual int GetExecutionCount()
        {
            return executionCount;
        }

        /// <summary>
        /// Start a coroutine which executes all commands in the Block. Only one running instance of each Block is permitted.
        /// </summary>
        public virtual void StartExecution()
        {
            StartCoroutine(Execute());
        }

        /// <summary>
        /// A coroutine method that executes all commands in the Block. Only one running instance of each Block is permitted.
        /// </summary>
        /// <param name="commandIndex">Index of command to start execution at</param>
        /// <param name="onComplete">Delegate function to call when execution completes</param>
        public virtual IEnumerator Execute(int commandIndex = 0, Action onComplete = null)
        {
            if (executionState != ExecutionState.Idle)
            {
                Debug.LogWarning(BlockName + " cannot be executed, it is already running.");
                yield break;
            }

            lastOnCompleteAction = onComplete;

            if (!executionInfoSet)
            {
                SetExecutionInfo();
            }

            executionCount++;
            var executionCountAtStart = executionCount;

            var flowchart = GetFlowchart();
            executionState = ExecutionState.Executing;
            BlockSignals.DoBlockStart(this);

            bool suppressSelectionChanges = false;

            #if UNITY_EDITOR
            // Select the executing block & the first command
            if (suppressAllAutoSelections || SuppressNextAutoSelection)
            {
                SuppressNextAutoSelection = false;
                suppressSelectionChanges = true;
            }
            else
            {
                flowchart.SelectedBlock = this;
                if (commandList.Count > 0)
                {
                    flowchart.ClearSelectedCommands();
                    flowchart.AddSelectedCommand(commandList[0]);
                }
            }
            #endif

            jumpToCommandIndex = commandIndex;

            int i = 0;
            while (true)
            {
                // Executing commands specify the next command to skip to by setting jumpToCommandIndex using Command.Continue()
                if (jumpToCommandIndex > -1)
                {
                    i = jumpToCommandIndex;
                    jumpToCommandIndex = -1;
                }

                // Skip disabled commands, comments and labels
                while (i < commandList.Count &&
                      (!commandList[i].enabled || 
                        commandList[i].GetType() == typeof(Comment) ||
                        commandList[i].GetType() == typeof(Label)))
                {
                    i = commandList[i].CommandIndex + 1;
                }

                if (i >= commandList.Count)
                {
                    break;
                }

                // The previous active command is needed for if / else / else if commands
                if (activeCommand == null)
                {
                    previousActiveCommandIndex = -1;
                }
                else
                {
                    previousActiveCommandIndex = activeCommand.CommandIndex;
                }

                var command = commandList[i];
                activeCommand = command;

                if (flowchart.IsActive() && !suppressSelectionChanges)
                {
                    // Auto select a command in some situations
                    if ((flowchart.SelectedCommands.Count == 0 && i == 0) ||
                        (flowchart.SelectedCommands.Count == 1 && flowchart.SelectedCommands[0].CommandIndex == previousActiveCommandIndex))
                    {
                        flowchart.ClearSelectedCommands();
                        flowchart.AddSelectedCommand(commandList[i]);
                    }
                }

                command.IsExecuting = true;
                // This icon timer is managed by the FlowchartWindow class, but we also need to
                // set it here in case a command starts and finishes execution before the next window update.
                command.ExecutingIconTimer = Time.realtimeSinceStartup + FungusConstants.ExecutingIconFadeTime;
                BlockSignals.DoCommandExecute(this, command, i, commandList.Count);
                command.Execute();

                // Wait until the executing command sets another command to jump to via Command.Continue()
                while (jumpToCommandIndex == -1)
                {
                    yield return null;
                }

                #if UNITY_EDITOR
                if (flowchart.StepPause > 0f)
                {
                    yield return new WaitForSeconds(flowchart.StepPause);
                }
                #endif

                command.IsExecuting = false;
            }

            if(State == ExecutionState.Executing &&
                //ensure we aren't dangling from a previous stopage and stopping a future run
                executionCountAtStart == executionCount)
            {
                ReturnToIdle();
            }
        }

        private void ReturnToIdle()
        {
            executionState = ExecutionState.Idle;
            activeCommand = null;
            BlockSignals.DoBlockEnd(this);

            if (lastOnCompleteAction != null)
            {
                lastOnCompleteAction();
            }
            lastOnCompleteAction = null;
        }

        /// <summary>
        /// Stop executing commands in this Block.
        /// </summary>
        public virtual void Stop()
        {
            // Tell the executing command to stop immediately
            if (activeCommand != null)
            {
                activeCommand.IsExecuting = false;
                activeCommand.OnStopExecuting();
            }

            // This will cause the execution loop to break on the next iteration
            jumpToCommandIndex = int.MaxValue;

            //force idle here so other commands that rely on block not executing are informed this frame rather than next
            ReturnToIdle();
        }

        /// <summary>
        /// Returns a list of all Blocks connected to this one.
        /// </summary>
        public virtual List<Block> GetConnectedBlocks()
        {
            var connectedBlocks = new List<Block>();
            GetConnectedBlocks(ref connectedBlocks);
            return connectedBlocks;
        }

        public virtual void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                if (command != null)
                {
                    command.GetConnectedBlocks(ref connectedBlocks);
                }
            }
        }

        /// <summary>
        /// Returns the type of the previously executing command.
        /// </summary>
        /// <returns>The previous active command type.</returns>
        public virtual System.Type GetPreviousActiveCommandType()
        {
            if (previousActiveCommandIndex >= 0 &&
                previousActiveCommandIndex < commandList.Count)
            {
                return commandList[previousActiveCommandIndex].GetType();
            }

            return null;
        }

        public virtual int GetPreviousActiveCommandIndent()
        {
            if (previousActiveCommandIndex >= 0 &&
                previousActiveCommandIndex < commandList.Count)
            {
                return commandList[previousActiveCommandIndex].IndentLevel;
            }

            return -1;
        }

        public virtual Command GetPreviousActiveCommand()
        {
            if (previousActiveCommandIndex >= 0 &&
                previousActiveCommandIndex < commandList.Count)
            {
                return commandList[previousActiveCommandIndex];
            }

            return null;
        }

        /// <summary>
        /// Recalculate the indent levels for all commands in the list.
        /// </summary>
        public virtual void UpdateIndentLevels()
        {
            int indentLevel = 0;
            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                if (command == null)
                {
                    continue;
                }
                if (command.CloseBlock())
                {
                    indentLevel--;
                }
                // Negative indent level is not permitted
                indentLevel = Math.Max(indentLevel, 0);
                command.IndentLevel = indentLevel;
                if (command.OpenBlock())
                {
                    indentLevel++;
                }
            }
        }

        /// <summary>
        /// Returns the index of the Label command with matching key, or -1 if not found.
        /// </summary>
        public virtual int GetLabelIndex(string labelKey)
        {
            if (labelKey.Length == 0)
            {
                return -1;
            }

            for (int i = 0; i < commandList.Count; i++)
            {
                var command = commandList[i];
                var labelCommand = command as Label;
                if (labelCommand != null && String.Compare(labelCommand.Key, labelKey, true) == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion
    }
}
