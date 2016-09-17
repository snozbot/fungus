// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Commands can be added to Blocks to create an execution sequence.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Unique identifier for this command.
        /// Unique for this Flowchart.
        /// </summary>
        int ItemId { get; set; }

        /// <summary>
        /// Error message to display in the command inspector.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Indent depth of the current commands.
        /// Commands are indented inside If, While, etc. sections.
        /// </summary>
        int IndentLevel { get; set; }

        /// <summary>
        /// Index of the command in the parent block's command list.
        /// </summary>
        int CommandIndex { get; set; }

        /// <summary>
        /// Set to true by the parent block while the command is executing.
        /// </summary>
        bool IsExecuting { get; set; }

        /// <summary>
        /// Timer used to control appearance of executing icon in inspector.
        /// </summary>
        float ExecutingIconTimer { get; set; }

        /// <summary>
        /// Reference to the Block object that this command belongs to.
        /// This reference is only populated at runtime and in the editor when the 
        /// block is selected.
        /// </summary>
        Block ParentBlock { get; set; }

        /// <summary>
        /// Returns the Flowchart that this command belongs to.
        /// </summary>
        IFlowchart GetFlowchart();

        /// <summary>
        /// Execute the command.
        /// </summary>
        void Execute();

        /// <summary>
        /// End execution of this command and continue execution at the next command.
        /// </summary>
        void Continue();

        /// <summary>
        /// End execution of this command and continue execution at a specific command index.
        /// </summary>
        /// <param name="nextCommandIndex">Next command index.</param>
        void Continue(int nextCommandIndex);

        /// <summary>
        /// Stops the parent Block executing.
        /// </summary>
        void StopParentBlock();

        /// <summary>
        /// Called when the parent block has been requested to stop executing, and
        /// this command is the currently executing command.
        /// Use this callback to terminate any asynchronous operations and 
        /// cleanup state so that the command is ready to execute again later on.
        /// </summary>
        void OnStopExecuting();

        /// <summary>
        /// Called when the new command is added to a block in the editor.
        /// </summary>
        void OnCommandAdded(Block parentBlock);

        /// <summary>
        /// Called when the command is deleted from a block in the editor.
        /// </summary>
        void OnCommandRemoved(Block parentBlock);

        /// <summary>
        /// Called when this command starts execution.
        /// </summary>
        void OnEnter();

        /// <summary>
        /// Called when this command ends execution.
        /// </summary>
        void OnExit();

        /// <summary>
        /// Called when this command is reset. This happens when the Reset command is used.
        /// </summary>
        void OnReset();

        /// <summary>
        /// Populates a list with the Blocks that this command references.
        /// </summary>
        void GetConnectedBlocks(ref List<Block> connectedBlocks);

        /// <summary>
        /// Returns true if this command references the variable.
        /// Used to highlight variables in the variable list when a command is selected.
        /// </summary>
        bool HasReference(Variable variable);

        /// <summary>
        /// Returns the summary text to display in the command inspector.
        /// </summary>
        string GetSummary();

        /// <summary>
        /// Returns the help text to display for this command.
        /// </summary>
        string GetHelpText();

        /// <summary>
        /// Return true if this command opens a block of commands. Used for indenting commands.
        /// </summary>
        bool OpenBlock();

        /// <summary>
        /// Return true if this command closes a block of commands. Used for indenting commands.
        /// </summary>
        bool CloseBlock();

        /// <summary>
        /// Return the color for the command background in inspector.
        /// </summary>
        /// <returns>The button color.</returns>
        Color GetButtonColor();

        /// <summary>
        /// Returns true if the specified property should be displayed in the inspector. 
        /// This is useful for hiding certain properties based on the value of another property.
        /// </summary>
        bool IsPropertyVisible(string propertyName);

        /// <summary>
        /// Returns true if the specified property should be displayed as a reorderable list in the inspector.
        /// This only applies for array properties and has no effect for non-array properties.
        /// </summary>
        bool IsReorderableArray(string propertyName);

        /// <summary>
        /// Returns the localization id for the Flowchart that contains this command.
        /// </summary>
        string GetFlowchartLocalizationId();
    }
}
