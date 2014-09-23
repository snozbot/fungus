using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Sequence : Node 
	{
		[System.NonSerialized]
		public Command activeCommand;

		public List<Command> commandList = new List<Command>();

		protected int executionCount;

		public virtual FungusScript GetFungusScript()
		{
			return GetComponentInParent<FungusScript>();
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
			fungusScript.selectedCommand = null;
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
