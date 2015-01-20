using UnityEngine;
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
		[HideInInspector]
		public string errorMessage = "";

		[HideInInspector]
		public int indentLevel;

		[NonSerialized]
		public int commandIndex;

		/**
		 * Reference to the Sequence object that this command belongs to.
		 * This reference is only set at runtime (null in editor).
		 */
		[NonSerialized]
		public Sequence parentSequence;

		public virtual FungusScript GetFungusScript()
		{
			FungusScript fungusScript = GetComponent<FungusScript>();
			if (fungusScript == null &&
			    transform.parent != null)
			{
				fungusScript = transform.parent.GetComponent<FungusScript>();
			}
			return fungusScript;
		}

		public virtual bool IsExecuting()
		{
			if (parentSequence == null)
			{
				return false;
			}

			return (parentSequence.activeCommand == this);
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
			if (parentSequence != null)
			{
				parentSequence.ExecuteCommand(nextCommandIndex);
			}
		}

		public virtual void Stop()
		{
			OnExit();
			if (parentSequence != null)
			{
				parentSequence.Stop();
			}
		}

		public virtual void ExecuteSequence(Sequence s)
		{
			OnExit();
			if (parentSequence != null)
			{
				FungusScript fungusScript = parentSequence.GetFungusScript();

				// Record the currently selected sequence because Stop() will clear it.
				Sequence selectedSequence = fungusScript.selectedSequence;

				parentSequence.Stop();
				if (fungusScript != null)
				{
					// If the executing sequence is currently selected then follow the execution 
					// onto the next sequence in the inspector.
					if (selectedSequence == parentSequence)
					{
						fungusScript.selectedSequence = s;
					}

					fungusScript.ExecuteSequence(s);
				}
			}
		}

		/**
		 * Called when the new command is added to a sequence in the editor.
		 */
		public virtual void OnCommandAdded(Sequence parentSequence)
		{}

		/**
		 * Called when the command is deleted from a sequence in the editor.
		 */
		public virtual void OnCommandRemoved(Sequence parentSequence)
		{}

		public virtual void OnEnter()
		{}

		public virtual void OnExit()
		{}

		public virtual void OnReset()
		{}

		public virtual void GetConnectedSequences(ref List<Sequence> connectedSequences)
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
	}

}