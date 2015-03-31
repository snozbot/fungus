using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(Flowchart))]
	[AddComponentMenu("")]
	public class Block : Node 
	{
		[FormerlySerializedAs("sequenceName")]
		public string blockName = "New Block";

		[TextArea(2, 5)]
		[Tooltip("Description text to display under the block node")]
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
			// Give each child command a reference back to its parent block
			// and tell each command its index in the list.
			int index = 0;
			foreach (Command command in commandList)
			{
				command.parentBlock = this;
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

		public virtual Flowchart GetFlowchart()
		{
			Flowchart flowchart = GetComponent<Flowchart>();

			if (flowchart == null)
			{
				// Legacy support for earlier system where Blocks were children of the Flowchart
				if (transform.parent != null)
				{
					flowchart = transform.parent.GetComponent<Flowchart>();
				}
			}

			return flowchart;
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
			Flowchart flowchart = GetFlowchart();
			if (flowchart == null)
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

			Flowchart flowchart = GetFlowchart();

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
				if (flowchart.gameObject.activeInHierarchy)
				{
					// Auto select a command in some situations
					if ((flowchart.selectedCommands.Count == 0 && commandIndex == 0) ||
					    (flowchart.selectedCommands.Count == 1 && flowchart.selectedCommands[0].commandIndex == previousActiveCommandIndex))
					{
						flowchart.ClearSelectedCommands();
						flowchart.AddSelectedCommand(nextCommand);
					}

					if (runSlowInEditor &&
					    nextCommand.RunSlowInEditor())
					{
						StartCoroutine(ExecuteAfterDelay(nextCommand, flowchart.runSlowDuration));
					}
					else
					{
						activeCommand = nextCommand;
						nextCommand.Execute();
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
			Flowchart flowchart = GetFlowchart();
			if (flowchart == null)
			{
				return;
			}

			activeCommand = null;
			flowchart.ClearSelectedCommands();
		}

		public virtual List<Block> GetConnectedBlocks()
		{
			List<Block> connectedBlocks = new List<Block>();
			foreach (Command command in commandList)
			{
				if (command != null)
				{
					command.GetConnectedBlocks(ref connectedBlocks);
				}
			}
			return connectedBlocks;
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
