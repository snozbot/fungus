using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Sequence : Node 
	{
		public string sequenceName = "New Sequence";

		[System.NonSerialized]
		public Command activeCommand;

		public List<Command> commandList = new List<Command>();

		protected int executionCount;

		protected virtual void Start()
		{
			// Give each child command a reference back to its parent sequence
			foreach (Command command in commandList)
			{
				command.parentSequence = this;
			}
		}

		public virtual FungusScript GetFungusScript()
		{
			FungusScript fungusScript = GetComponent<FungusScript>();

			if (fungusScript == null)
			{
				// Legacy support for earlier system where Sequences were children of the FungusScript
				if (transform.parent != null)
				{
					fungusScript = transform.parent.GetComponent<FungusScript>();
				}
			}

			return fungusScript;
		}

		public virtual bool HasError()
		{
			foreach (Command command in commandList)
			{
				if (command.errorMessage.Length > 0)
				{
					return true;
				}
			}

			return false;
		}

		public virtual bool IsRunning()
		{
			FungusScript fungusScript = GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.executingSequence == null)
			{
				return false;
			}

			return (fungusScript.executingSequence == this);
		}

		public virtual int GetExecutionCount()
		{
			return executionCount;
		}

		public virtual void ExecuteNextCommand(Command currentCommand = null)
		{
			if (currentCommand == null)
			{
				executionCount++;
			}

			activeCommand = null;
			Command nextCommand = null;

			bool executeNext = (currentCommand == null);
			foreach (Command command in commandList)
			{
				if (command == currentCommand)
				{
					executeNext = true;
				}
				else if (executeNext)
				{
					if (command.enabled)
					{
						nextCommand = command;
						break;
					}
				}
			}

			if (nextCommand == null)
			{
				Stop();
			}
			else
			{
				FungusScript fungusScript = GetFungusScript();

				if (!fungusScript.settings.runSlowInEditor)
				{
					activeCommand = nextCommand;
					nextCommand.Execute();
				}
				else
				{
					StartCoroutine(ExecuteAfterDelay(nextCommand, fungusScript.settings.runSlowDuration));
				}
			}

		}

		IEnumerator ExecuteAfterDelay(Command command, float delay)
		{
			activeCommand = command;
			yield return new WaitForSeconds(delay);
			command.Execute();
		}

		public virtual void Stop()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			activeCommand = null;
			fungusScript.executingSequence = null;
			fungusScript.selectedSequence = null;
			fungusScript.ClearSelectedCommands();
		}

		public virtual List<Sequence> GetConnectedSequences()
		{
			List<Sequence> connectedSequences = new List<Sequence>();
			foreach (Command command in commandList)
			{
				command.GetConnectedSequences(ref connectedSequences);
			}
			return connectedSequences;
		}

		// Force set the sequence name for any legacy child sequences.
		// This is a temporary hack to make it easier to upgrade from earlier versions and will be removed soon.
		public virtual void UpdateSequenceName()
		{
			if (sequenceName == "New Sequence" &&
			    GetComponent<FungusScript>() == null)
			{
				sequenceName = gameObject.name;
			}
		}
	}
}
