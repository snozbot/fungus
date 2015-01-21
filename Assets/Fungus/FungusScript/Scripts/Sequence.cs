using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(FungusScript))]
	[AddComponentMenu("")]
	public class Sequence : Node 
	{
		public string sequenceName = "Sequence";

		[TextArea(2, 5)]
		[Tooltip("Description text to display under the sequence node")]
		public string description = "";

		[Tooltip("Slow down execution in the editor to make it easier to visualise program flow")]
		public bool runSlowInEditor = true;

		public EventHandler eventHandler;

		[HideInInspector]
		[System.NonSerialized]
		public Command activeCommand;

		// Index of last command executed before the current one
		// -1 indicates no previous command
		[HideInInspector]
		[System.NonSerialized]
		public int previousActiveCommandIndex = -1;

		[HideInInspector]
		[System.NonSerialized]
		public float executingIconTimer;

		[HideInInspector]
		public List<Command> commandList = new List<Command>();

		protected int executionCount;

		protected virtual void Awake()
		{
			// Give each child command a reference back to its parent sequence
			// and tell each command its index in the list.
			int index = 0;
			foreach (Command command in commandList)
			{
				command.parentSequence = this;
				command.commandIndex = index++;
			}
		}

#if UNITY_EDITOR
		// The user can modify the command list order while playing in the editor,
		// so we keep the command indices updated every frame. There's no need to
		// do this in player builds so we compile this bit out for those builds.
		void Update()
		{
			int index = 0;
			foreach (Command command in commandList)
			{
				if (command == null) // Null entry will be deleted automatically later
				{
					continue;
				}

				command.commandIndex = index++;
			}
		}
#endif

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

		public virtual void ExecuteCommand(int commandIndex)
		{
			if (activeCommand == null)
			{
				previousActiveCommandIndex = -1;
			}
			else
			{
				previousActiveCommandIndex = activeCommand.commandIndex;
			}

			if (commandIndex >= commandList.Count)
			{
				Stop();
				return;
			}

			if (commandIndex == 0)
			{
				executionCount++;
			}

			FungusScript fungusScript = GetFungusScript();

			// Skip disabled commands, comments and labels
			while (commandIndex < commandList.Count &&
				   (!commandList[commandIndex].enabled || 
			 		commandList[commandIndex].GetType() == typeof(Comment) ||
			 	    commandList[commandIndex].GetType() == typeof(Label)))
			{
				commandIndex = commandList[commandIndex].commandIndex + 1;
			}

			if (commandIndex >= commandList.Count)
			{
				Stop();
				return;
			}

			Command nextCommand = commandList[commandIndex];

			activeCommand = null;
			executingIconTimer = 0.5f;

			if (nextCommand == null)
			{
				Stop();
			}
			else
			{
				if (fungusScript.gameObject.activeInHierarchy)
				{
					// Auto select a command in some situations
					if ((fungusScript.selectedCommands.Count == 0 && commandIndex == 0) ||
					    (fungusScript.selectedCommands.Count == 1 && fungusScript.selectedCommands[0].commandIndex == previousActiveCommandIndex))
					{
						fungusScript.ClearSelectedCommands();
						fungusScript.AddSelectedCommand(nextCommand);
					}

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

		IEnumerator ExecuteAfterDelay(Command nextCommand, float delay)
		{
			activeCommand = nextCommand;
			yield return new WaitForSeconds(delay);
			nextCommand.Execute();
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

		public virtual System.Type GetPreviousActiveCommandType()
		{
			if (previousActiveCommandIndex >= 0 &&
			    previousActiveCommandIndex < commandList.Count)
			{
				return commandList[previousActiveCommandIndex].GetType();
			}

			return null;
		}
	}
}
