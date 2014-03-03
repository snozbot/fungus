using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/**
	 * Manages a sequential list of commands.
	 * When a command completes, the next command is popped from the queue and exectuted.
	 */
	public class CommandQueue : MonoBehaviour
	{
		/**
		 * Base class for commands used with the CommandQueue.
		 */
		public abstract class Command
		{
			public abstract void Execute(CommandQueue commandQueue, Action onComplete);
		}

		List<Command> commandList = new List<Command>();

		/**
		 * Adds a command to the queue for later execution
		 * @param command A command object to add to the queue
		 */
		public virtual void AddCommand(Command command)
		{
			commandList.Add(command);
		}

		/**
		 * Clears all queued commands from the list
		 */
		public virtual void Reset()
		{
			commandList.Clear();
		}

		/** 
		 * Executes the first command in the queue.
		 * When this command completes, the next command in the queue is executed.
		 */
		public virtual void Execute()
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

		/**
		 * Calls a named method on a game object to populate the command queue.
		 * The command queue is then executed.
		 */
		public void CallCommandMethod(GameObject target, string methodName)
		{
			Reset();
			target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
			Execute();
		}
		
		/**
		 * Calls an Action delegate method to populate the command queue.
		 * The command queue is then executed.
		 */
		public void CallCommandMethod(Action method)
		{
			Reset();
			if (method != null)
			{
				method();
			}
			Execute();
		}
	}
}