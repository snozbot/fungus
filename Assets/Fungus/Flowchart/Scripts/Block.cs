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
		public enum ExecutionState
		{
			Idle,
			Executing,
		}

		[NonSerialized]
		public ExecutionState executionState;

		[HideInInspector]
		public int itemId = -1; // Invalid flowchart item id

		[FormerlySerializedAs("sequenceName")]
		[Tooltip("The name of the block node as displayed in the Flowchart window")]
		public string blockName = "New Block";

		[TextArea(2, 5)]
		[Tooltip("Description text to display under the block node")]
		public string description = "";

		[Tooltip("An optional Event Handler which can execute the block when an event occurs")]
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

		/**
		 * Duration of fade for executing icon displayed beside blocks & commands.
		 */
		public const float executingIconFadeTime = 0.5f;

		/**
		 * Controls the next command to execute in the block execution coroutine.
		 */
		[NonSerialized]
		public int jumpToCommandIndex = -1;

		protected bool executionInfoSet = false;

		protected virtual void Awake()
		{
			SetExecutionInfo();
		}

		protected virtual void SetExecutionInfo()
		{
			// Give each child command a reference back to its parent block
			// and tell each command its index in the list.
			int index = 0;
			foreach (Command command in commandList)
			{
				if (command == null)
				{
					continue;
				}

				command.parentBlock = this;
				command.commandIndex = index++;
			}

			executionInfoSet = true;
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
			return GetComponent<Flowchart>();
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
			return (executionState == ExecutionState.Executing);
		}

		public virtual int GetExecutionCount()
		{
			return executionCount;
		}

		public virtual bool Execute(Action onComplete = null)
		{
			if (executionState != ExecutionState.Idle)
			{
				return false;
			}

			if (!executionInfoSet)
			{
				SetExecutionInfo();
			}

			executionCount++;
			StartCoroutine(ExecuteBlock(onComplete));

			return true;
		}

		protected virtual IEnumerator ExecuteBlock(Action onComplete = null)
		{
			Flowchart flowchart = GetFlowchart();
			executionState = ExecutionState.Executing;

			#if UNITY_EDITOR
			// Select the executing block & the first command
			flowchart.selectedBlock = this;
			if (commandList.Count > 0)
			{
				flowchart.ClearSelectedCommands();
				flowchart.AddSelectedCommand(commandList[0]);
			}
			#endif

			int i = 0;
			while (true)
			{
				// Executing commands specify the next command to skip to by setting jumpToCommandIndex using Command.Continue()
				if (jumpToCommandIndex > -1)
				{
					i = jumpToCommandIndex;
					jumpToCommandIndex = -1;
				}

				// Skip disabled commands, comments and labels
				while (i < commandList.Count &&
				       (!commandList[i].enabled || 
				 		commandList[i].GetType() == typeof(Comment) ||
				 		commandList[i].GetType() == typeof(Label)))
				{
					i = commandList[i].commandIndex + 1;
				}

				if (i >= commandList.Count)
				{
					break;
				}

				// The previous active command is needed for if / else / else if commands
				if (activeCommand == null)
				{
					previousActiveCommandIndex = -1;
				}
				else
				{
					previousActiveCommandIndex = activeCommand.commandIndex;
				}

				Command command = commandList[i];
				activeCommand = command;

				if (flowchart.gameObject.activeInHierarchy)
				{
					// Auto select a command in some situations
					if ((flowchart.selectedCommands.Count == 0 && i == 0) ||
					    (flowchart.selectedCommands.Count == 1 && flowchart.selectedCommands[0].commandIndex == previousActiveCommandIndex))
					{
						flowchart.ClearSelectedCommands();
						flowchart.AddSelectedCommand(commandList[i]);
					}
				}

				command.isExecuting = true;
				// This icon timer is managed by the FlowchartWindow class, but we also need to
				// set it here in case a command starts and finishes execution before the next window update.
				command.executingIconTimer = Time.realtimeSinceStartup + executingIconFadeTime;
				command.Execute();

				// Wait until the executing command sets another command to jump to via Command.Continue()
				while (jumpToCommandIndex == -1)
				{
					yield return null;
				}

				#if UNITY_EDITOR
				if (flowchart.stepPause > 0f)
				{
					yield return new WaitForSeconds(flowchart.stepPause);
				}
				#endif

				command.isExecuting = false;
			}

			executionState = ExecutionState.Idle;
			activeCommand = null;

			if (onComplete != null)
			{
				onComplete();
			}
		}

		public virtual void Stop()
		{
			// Tell the executing command to stop immediately
			if (activeCommand != null)
			{
				activeCommand.isExecuting = false;
				activeCommand.OnStopExecuting();
			}

			// This will cause the execution loop to break on the next iteration
			jumpToCommandIndex = int.MaxValue;
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
