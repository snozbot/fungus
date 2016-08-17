/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    
    public class CommandInfoAttribute : Attribute
    {
        /**
         * Metadata atribute for the Command class.
         * @param category The category to place this command in.
         * @param commandName The display name of the command.
         * @param helpText Help information to display in the inspector.
         * @param priority If two command classes have the same name, the one with highest priority is listed. Negative priority removess the command from the list.
         */
        public CommandInfoAttribute(string category, string commandName, string helpText, int priority = 0)
        {
            this.Category = category;
            this.CommandName = commandName;
            this.HelpText = helpText;
            this.Priority = priority;
        }

        public string Category { get; set; }
        public string CommandName { get; set; }
        public string HelpText { get; set; }
        public int Priority { get; set; }
    }

    public class Command : MonoBehaviour
    {
        [FormerlySerializedAs("commandId")]
        [HideInInspector]
        public int itemId = -1; // Invalid flowchart item id

        [HideInInspector]
        public string errorMessage = "";

        [HideInInspector]
        public int indentLevel;

        [NonSerialized]
        public int commandIndex;

        /**
         * Set to true by the parent block while the command is executing.
         */
        [NonSerialized]
        public bool isExecuting;

        /**
         * Timer used to control appearance of executing icon in inspector.
         */
        [NonSerialized]
        public float executingIconTimer;

        /**
         * Reference to the Block object that this command belongs to.
         * This reference is only populated at runtime and in the editor when the 
         * block is selected.
         */
        [NonSerialized]
        public Block parentBlock;

        public virtual Flowchart GetFlowchart()
        {
            Flowchart flowchart = GetComponent<Flowchart>();
            if (flowchart == null &&
                transform.parent != null)
            {
                flowchart = transform.parent.GetComponent<Flowchart>();
            }
            return flowchart;
        }

        public virtual void Execute()
        {
            OnEnter();
        }

        public virtual void Continue()
        {
            // This is a noop if the Block has already been stopped
            if (isExecuting)
            {
                Continue(commandIndex + 1);
            }
        }

        public virtual void Continue(int nextCommandIndex)
        {
            OnExit();
            if (parentBlock != null)
            {
                parentBlock.jumpToCommandIndex = nextCommandIndex;
            }
        }

        public virtual void StopParentBlock()
        {
            OnExit();
            if (parentBlock != null)
            {
                parentBlock.Stop();
            }
        }

        /**
         * Called when the parent block has been requested to stop executing, and
         * this command is the currently executing command.
         * Use this callback to terminate any asynchronous operations and 
         * cleanup state so that the command is ready to execute again later on.
         */
        public virtual void OnStopExecuting()
        {}

        /**
         * Called when the new command is added to a block in the editor.
         */
        public virtual void OnCommandAdded(Block parentBlock)
        {}

        /**
         * Called when the command is deleted from a block in the editor.
         */
        public virtual void OnCommandRemoved(Block parentBlock)
        {}

        public virtual void OnEnter()
        {}

        public virtual void OnExit()
        {}

        public virtual void OnReset()
        {}

        public virtual void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {}

        public virtual bool HasReference(Variable variable)
        {
            return false;
        }

        public virtual string GetSummary()
        {
            return "";
        }

        public virtual string GetHelpText()
        {
            return "";
        }

        /**
         * This command starts a block of commands.
         */
        public virtual bool OpenBlock()
        {
            return false;
        }

        /**
         * This command ends a block of commands.
         */
        public virtual bool CloseBlock()
        {
            return false;
        }

        /**
         * Return the color for the command background in inspector.
         */
        public virtual Color GetButtonColor()
        {
            return Color.white;
        }

        /**
         * Returns true if the specified property should be displayed in the inspector.
         * This is useful for hiding certain properties based on the value of another property.
         */
        public virtual bool IsPropertyVisible(string propertyName)
        {
            return true;
        }

        /**
         * Returns true if the specified property should be displayed as a reorderable list in the inspector.
         * This only applies for array properties and has no effect for non-array properties.
         */
        public virtual bool IsReorderableArray(string propertyName)
        {
            return false;
        }

        /**
         * Returns the localization id for the Flowchart that contains this command.
         */
        public virtual string GetFlowchartLocalizationId()
        {
            // If no localization id has been set then use the Flowchart name
            Flowchart flowchart = GetFlowchart();
            if (flowchart == null)
            {
                return "";
            }

            string localizationId = GetFlowchart().localizationId;
            if (localizationId.Length == 0)
            {
                localizationId = flowchart.name;            
            }

            return localizationId;
        }

    }

}