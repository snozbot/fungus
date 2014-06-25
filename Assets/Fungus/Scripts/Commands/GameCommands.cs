using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 *  Command classes have their own namespace to prevent them popping up in code completion.
	 */
	namespace Command
	{
		/** 
		 * Call a delegate method on execution.
		 * This command can be used to schedule arbitrary script code.
		 */
		public class Call : CommandQueue.Command
		{
			Action onExecute;
			
			public Call(Action callAction)
			{
				if (callAction == null)
				{
					Debug.LogError("Action must not be null.");
					return;
				}
				
				onExecute = callAction;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (onExecute != null)
					onExecute();
				
				if (onComplete != null)
					onComplete();	
			}		
		}
		
		/**
		 * Wait for a period of time.
		 */
		public class Wait : CommandQueue.Command
		{
			float duration;
			
			public Wait(float _duration)
			{
				duration = _duration;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game.GetInstance().waiting = true;
				commandQueue.StartCoroutine(WaitCoroutine(duration, onComplete));
			}
			
			IEnumerator WaitCoroutine(float duration, Action onComplete)
			{
				yield return new WaitForSeconds(duration);
				if (onComplete != null)
				{
					Game.GetInstance().waiting = false;
					onComplete();
				}
			}
		}
		
		/**
		 * Wait for a player tap/click/key press
		 */
		public class WaitForInput : CommandQueue.Command
		{
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game.GetInstance().waiting = true;
				commandQueue.StartCoroutine(WaitCoroutine(onComplete));
			}
			
			IEnumerator WaitCoroutine(Action onComplete)
			{
				while (true)
				{
					if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
					{
						break;
					}
					yield return null;
				}
				
				if (onComplete != null)
				{
					Game.GetInstance().waiting = false;
					onComplete();
				}
			}
		}

		/** 
		 * Changes the active room to a different room
		 */
		public class MoveToRoom : CommandQueue.Command
		{
			Action onExecute;

			public MoveToRoom(Room room)
			{
				if (room == null)
				{
					Debug.LogError("Room must not be null.");
					return;
				}

				onExecute = delegate {
					Game game = Game.GetInstance();
					game.waiting = true;
					
					// Fade out screen
					game.cameraController.Fade(0f, game.roomFadeDuration / 2f, delegate {
						
						game.activeRoom = room;
						
						// Notify room script that the Room is being entered
						// Calling private method on Room to hide implementation
						game.activeRoom.gameObject.SendMessage("Enter");
						
						// Fade in screen
						game.cameraController.Fade(1f, game.roomFadeDuration / 2f, delegate {
							game.waiting = false;
						});
					});
				};
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (onExecute != null)
					onExecute();

				// This command resets the command queue so no need to call onComplete
			}
		}

		/** 
		 * Switches to a different scene.
		 */
		public class MoveToScene : CommandQueue.Command
		{
			Action onExecute;

			public MoveToScene(string sceneName)
			{
				if (sceneName == "")
				{
					Debug.LogError("Scene name must not be empty");
					return;
				}

				onExecute = delegate {
					Game game = Game.GetInstance();
					game.waiting = true;
					
					// Fade out screen
					game.cameraController.Fade(0f, game.roomFadeDuration / 2f, delegate {
						Game.GetInstance().LoadScene(sceneName, true);
					});
				};
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (onExecute != null)
					onExecute();

				// This command resets the command queue so no need to call onComplete
			}
		}
	}
}