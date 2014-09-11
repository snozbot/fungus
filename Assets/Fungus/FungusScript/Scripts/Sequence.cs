#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class Sequence : MonoBehaviour 
	{
		public Rect nodeRect = new Rect(10, 10, 100, 40);

		public string description = "";

		[System.NonSerialized]
		public Command activeCommand;

		public List<Command> commandList = new List<Command>();

		int executionCount;

		public FungusScript GetFungusScript()
		{
			return GetComponentInParent<FungusScript>();
		}

		public bool HasError()
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

		public bool IsRunning()
		{
			FungusScript fungusScript = GetFungusScript();

			if (fungusScript == null ||
			    fungusScript.executingSequence == null)
			{
				return false;
			}

			return (fungusScript.executingSequence == this);
		}

		public int GetExecutionCount()
		{
			return executionCount;
		}

		public void ExecuteNextCommand(Command currentCommand = null)
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

				if (fungusScript.stepTime == 0f)
				{
					activeCommand = nextCommand;
					nextCommand.Execute();
				}
				else
				{
					StartCoroutine(ExecuteAfterDelay(nextCommand, fungusScript.stepTime));
				}
			}

		}

		IEnumerator ExecuteAfterDelay(Command command, float delay)
		{
			activeCommand = command;
			yield return new WaitForSeconds(delay);
			command.Execute();
		}

		public void Stop()
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

		public List<Sequence> GetConnectedSequences()
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
