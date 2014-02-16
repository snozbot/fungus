using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Manages a sequential list of commands.
	// When a command completes, the next command is popped from the queue and exectuted.
	public class CommandQueue : MonoBehaviour
	{
		[HideInInspector]
		public CameraController cameraController;

		public void Start()
		{
			cameraController = Game.GetInstance().GetComponent<CameraController>();
		}

		// Base class for commands used with the CommandQueue
		public abstract class Command
		{
			public abstract void Execute(CommandQueue commandQueue, Action onComplete);
		}

		List<Command> commandList = new List<Command>();

		public void AddCommand(Command command)
		{
			commandList.Add(command);
		}

		public void Reset()
		{
			StopAllCoroutines();
			commandList.Clear();
		}

		public void Execute()
		{
			if (commandList.Count == 0)
			{
				return;
			}

			Command command = commandList[0];

			command.Execute(this, delegate {
				commandList.RemoveAt(0);
				Execute();
			});
		}
	}
}