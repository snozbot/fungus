using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	[RequireComponent(typeof(FungusScript))]
	public class Sequence : Node 
	{
		public string sequenceName = "New Sequence";

		[Tooltip("Slow down execution in the editor to make it easier to visualise program flow")]
		public bool runSlowInEditor = true;

		public EventHandler eventHandler;

		[HideInInspector]
		[System.NonSerialized]
		public Command activeCommand;

		[HideInInspector]
		public List<Command> commandList = new List<Command>();

		protected int executionCount;

		protected virtual void Awake()
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

		public virtual bool IsExecuting()
		{
			FungusScript fungusScript = GetFungusScript();
			if (fungusScript == null)
			{
				return false;
			}

			return (activeCommand != null);
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
					if (command.enabled && command.GetType() != typeof(Comment))
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

				if (fungusScript.gameObject.activeInHierarchy)
				{
					if (!runSlowInEditor)
					{
						activeCommand = nextCommand;
						nextCommand.Execute();
					}
					else
					{
						StartCoroutine(ExecuteAfterDelay(nextCommand, fungusScript.runSlowDuration));
					}
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
	}
}
