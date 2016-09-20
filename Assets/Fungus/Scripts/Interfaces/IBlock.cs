// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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
    public interface IBlock
    {
        /// <summary>
        /// The execution state of the Block.
        /// </summary>
        ExecutionState State { get; }

        /// <summary>
        /// Unique identifier for the Block.
        /// </summary>
        int ItemId { get; set; }

        /// <summary>
        /// The name of the block node as displayed in the Flowchart window.
        /// </summary>
        string BlockName { get; set; }

        /// <summary>
        /// Description text to display under the block node
        /// </summary>
        string Description { get; }

        /// <summary>
        /// An optional Event Handler which can execute the block when an event occurs.
        /// Note: Using the concrete class instead of the interface here because of weird editor behaviour.
        /// </summary>
        EventHandler _EventHandler { get; set; }

        /// <summary>
        /// The currently executing command.
        /// </summary>
        Command ActiveCommand { get; }

        /// <summary>
        /// Timer for fading Block execution icon.
        /// </summary>
        float ExecutingIconTimer { get; set; }

        /// <summary>
        /// The list of commands in the sequence.
        /// </summary>
        List<Command> CommandList { get; }

        /// <summary>
        /// Controls the next command to execute in the block execution coroutine.
        /// </summary>
        int JumpToCommandIndex { set; }

        /// <summary>
        /// Returns the parent Flowchart for this Block.
        /// </summary>
        IFlowchart GetFlowchart();

        /// <summary>
        /// Returns true if the Block is executing a command.
        /// </summary>
        bool IsExecuting();

        /// <summary>
        /// Returns the number of times this Block has executed.
        /// </summary>
        int GetExecutionCount();

        /// <summary>
        /// Start a coroutine which executes all commands in the Block. Only one running instance of each Block is permitted.
        /// </summary>
        void StartExecution();

        /// <summary>
        /// A coroutine method that executes all commands in the Block. Only one running instance of each Block is permitted.
        /// </summary>
        /// <param name="commandIndex">Index of command to start execution at</param>
        /// <param name="onComplete">Delegate function to call when execution completes</param>
        IEnumerator Execute(int commandIndex = 0, System.Action onComplete = null);

        /// <summary>
        /// Stop executing commands in this Block.
        /// </summary>
        void Stop();

        /// <summary>
        /// Returns a list of all Blocks connected to this one.
        /// </summary>
        List<Block> GetConnectedBlocks();

        /// <summary>
        /// Returns the type of the previously executing command.
        /// </summary>
        /// <returns>The previous active command type.</returns>
        System.Type GetPreviousActiveCommandType();

        /// <summary>
        /// Recalculate the indent levels for all commands in the list.
        /// </summary>
        void UpdateIndentLevels();
    }
}