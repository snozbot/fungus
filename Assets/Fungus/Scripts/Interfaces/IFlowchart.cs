// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using Fungus.Commands;

namespace Fungus
{
    /// <summary>
    /// Visual scripting controller for the Flowchart programming language.
    /// Flowchart objects may be edited visually using the Flowchart editor window.
    /// </summary>
    public interface IFlowchart 
    {
        /// <summary>
        /// Scroll position of Flowchart editor window.
        /// </summary>
        Vector2 ScrollPos { get; set; }

        /// <summary>
        /// Scroll position of Flowchart variables window.
        /// </summary>
        Vector2 VariablesScrollPos { get; set; }

        /// <summary>
        /// Show the variables pane.
        /// </summary>
        bool VariablesExpanded { get; set; }

        /// <summary>
        /// Height of command block view in inspector.
        /// </summary>
        float BlockViewHeight { get; set; }

        /// <summary>
        /// Zoom level of Flowchart editor window.
        /// </summary>
        float Zoom { get; set; }

        /// <summary>
        /// Scrollable area for Flowchart editor window.
        /// </summary>
        Rect ScrollViewRect { get; set; }

        /// <summary>
        /// Currently selected block in the Flowchart editor.
        /// </summary>
        Block SelectedBlock { get; set; }

        /// <summary>
        /// Currently selected command in the Flowchart editor.
        /// </summary>
        List<Command> SelectedCommands { get; }

        /// <summary>
        /// The list of variables that can be accessed by the Flowchart.
        /// </summary>
        List<Variable> Variables { get; }

        /// <summary>
        /// Description text displayed in the Flowchart editor window
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Slow down execution in the editor to make it easier to visualise program flow.
        /// </summary>
        float StepPause { get; }

        /// <summary>
        /// Use command color when displaying the command list in the inspector.
        /// </summary>
        bool ColorCommands { get; }

        /// <summary>
        /// Saves the selected block and commands when saving the scene. Helps avoid version control conflicts if you've only changed the active selection.
        /// </summary>
        bool SaveSelection { get; }

        /// <summary>
        /// Unique identifier for identifying this flowchart in localized string keys.
        /// </summary>
        string LocalizationId { get; }

        /// <summary>
        /// Display line numbers in the command list in the Block inspector.
        /// </summary>
        bool ShowLineNumbers { get; }

        /// <summary>
        /// Lua Environment to be used by default for all Execute Lua commands in this Flowchart.
        /// </summary>
        ILuaEnvironment LuaEnv { get; }

        /// <summary>
        /// The ExecuteLua command adds a global Lua variable with this name bound to the flowchart prior to executing.
        /// </summary>
        string LuaBindingName { get; }

        /// <summary>
        /// Position in the center of all blocks in the flowchart.
        /// </summary>
        Vector2 CenterPosition { set; get; }

        /// <summary>
        /// Variable to track flowchart's version so components can update to new versions.
        /// </summary>
        int Version { set; }

        /// <summary>
        /// Returns the next id to assign to a new flowchart item.
        /// Item ids increase monotically so they are guaranteed to
        /// be unique within a Flowchart.
        /// </summary>
        int NextItemId();

        /// <summary>
        /// Returns true if the Flowchart gameobject is active.
        /// </summary>
        bool IsActive();

        /// <summary>
        /// Returns the Flowchart gameobject name.
        /// </summary>
        string GetName();

        /// <summary>
        /// Create a new block node which you can then add commands to.
        /// </summary>
        Block CreateBlock(Vector2 position);

        /// <summary>
        /// Returns the named Block in the flowchart, or null if not found.
        /// </summary>
        IBlock FindBlock(string blockName);

        /// <summary>
        /// Execute a child block in the Flowchart.
        /// You can use this method in a UI event. e.g. to handle a button click.
        void ExecuteBlock(string blockName);

        /// <summary>
        /// Execute a child block in the flowchart.
        /// The block must be in an idle state to be executed.
        /// This version provides extra options to control how the block is executed.
        /// Returns true if the Block started execution.            
        /// </summary>
        bool ExecuteBlock(IBlock block, int commandIndex = 0, System.Action onComplete = null);

        /// <summary>
        /// Stop all executing Blocks in this Flowchart.
        /// </summary>
        void StopAllBlocks();

        /// <summary>
        /// Sends a message to this Flowchart only.
        /// Any block with a matching MessageReceived event handler will start executing.
        /// </summary>
        void SendFungusMessage(string messageName);

        /// <summary>
        /// Returns a new variable key that is guaranteed not to clash with any existing variable in the list.
        /// </summary>
        string GetUniqueVariableKey(string originalKey, Variable ignoreVariable = null);

        /// <summary>
        /// Returns a new Block key that is guaranteed not to clash with any existing Block in the Flowchart.
        /// </summary>
        string GetUniqueBlockKey(string originalKey, IBlock ignoreBlock = null);

        /// <summary>
        /// Returns a new Label key that is guaranteed not to clash with any existing Label in the Block.
        /// </summary>
        string GetUniqueLabelKey(string originalKey, Label ignoreLabel);

        /// <summary>
        /// Returns the variable with the specified key, or null if the key is not found.
        /// You will need to cast the returned variable to the correct sub-type.
        /// You can then access the variable's value using the Value property. e.g.
        /// BooleanVariable boolVar = flowchart.GetVariable("MyBool") as BooleanVariable;
        /// boolVar.Value = false;
        /// </summary>
        Variable GetVariable(string key);

        /// <summary>
        /// Returns the variable with the specified key, or null if the key is not found.
        /// You can then access the variable's value using the Value property. e.g.
        /// BooleanVariable boolVar = flowchart.GetVariable<BooleanVariable>("MyBool");
        /// boolVar.Value = false;
        /// </summary>
        T GetVariable<T>(string key) where T : Variable;

        /// <summary>
        /// Register a new variable with the Flowchart at runtime. 
        /// The variable should be added as a component on the Flowchart game object.
        /// </summary>
        void SetVariable<T>(string key, T newvariable) where T : Variable;

        /// <summary>
        /// Gets a list of all variables with public scope in this Flowchart.
        /// </summary>
        List<Variable> GetPublicVariables();

        /// <summary>
        /// Gets the value of a boolean variable.
        /// Returns false if the variable key does not exist.
        /// </summary>
        bool GetBooleanVariable(string key);

        /// <summary>
        /// Sets the value of a boolean variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        void SetBooleanVariable(string key, bool value);

        /// <summary>
        /// Gets the value of an integer variable.
        /// Returns 0 if the variable key does not exist.
        /// </summary>
        int GetIntegerVariable(string key);

        /// <summary>
        /// Sets the value of an integer variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        void SetIntegerVariable(string key, int value);

        /// <summary>
        /// Gets the value of a float variable.
        /// Returns 0 if the variable key does not exist.
        /// </summary>
        float GetFloatVariable(string key);

        /// <summary>
        /// Sets the value of a float variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        void SetFloatVariable(string key, float value);

        /// <summary>
        /// Gets the value of a string variable.
        /// Returns the empty string if the variable key does not exist.
        /// </summary>
        string GetStringVariable(string key);

        /// <summary>
        /// Sets the value of a string variable.
        /// The variable must already be added to the list of variables for this Flowchart.
        /// </summary>
        void SetStringVariable(string key, string value);

        /// <summary>
        /// Set the block objects to be hidden or visible depending on the hideComponents property.
        /// </summary>
        void UpdateHideFlags();

        /// <summary>
        /// Clears the list of selected commands.
        /// </summary>
        void ClearSelectedCommands();

        /// <summary>
        /// Adds a command to the list of selected commands.
        /// </summary>
        void AddSelectedCommand(Command command);

        /// <summary>
        /// Reset the commands and variables in the Flowchart.
        /// </summary>
        void Reset(bool resetCommands, bool resetVariables);

        /// <summary>
        /// Override this in a Flowchart subclass to filter which commands are shown in the Add Command list.
        /// </summary>
        bool IsCommandSupported(CommandInfoAttribute commandInfo);

        /// <summary>
        /// Returns true if there are any executing blocks in this Flowchart.
        /// </summary>
        bool HasExecutingBlocks();

        /// <summary>
        /// Returns a list of all executing blocks in this Flowchart.
        /// </summary>
        List<IBlock> GetExecutingBlocks();

        /// <summary>
        /// Substitute variables in the input text with the format {$VarName}
        /// This will first match with private variables in this Flowchart, and then
        /// with public variables in all Flowcharts in the scene (and any component
        /// in the scene that implements StringSubstituter.ISubstitutionHandler).
        /// </summary>
        string SubstituteVariables(string input);
    }
}
