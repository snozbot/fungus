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

		public virtual bool IsExecuting()
		{
			if (parentBlock == null)
			{
				return false;
			}

			return (parentBlock.activeCommand == this);
		}

		public virtual void Execute()
		{
			OnEnter();
		}

		public virtual void Continue()
		{
			Continue(commandIndex + 1);
		}

		public virtual void Continue(int nextCommandIndex)
		{
			OnExit();
			if (parentBlock != null)
			{
				parentBlock.ExecuteCommand(nextCommandIndex);
			}
		}

		public virtual void Stop()
		{
			OnExit();
			if (parentBlock != null)
			{
				parentBlock.Stop();
			}
		}

		public virtual void ExecuteBlock(Block s)
		{
			OnExit();
			if (parentBlock != null)
			{
				Flowchart flowchart = parentBlock.GetFlowchart();

				// Record the currently selected block because Stop() will clear it.
				Block selectedBlock = flowchart.selectedBlock;

				parentBlock.Stop();
				if (flowchart != null)
				{
					// If the executing block is currently selected then follow the execution 
					// onto the next block in the inspector.
					if (selectedBlock == parentBlock)
					{
						flowchart.selectedBlock = s;
					}

					flowchart.ExecuteBlock(s);
				}
			}
		}

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
		 * Adds a delay between commands if the 'Run Slow In Editor' option is enabled.
		 */
		public virtual bool RunSlowInEditor()
		{
			return true;
		}
	}

}