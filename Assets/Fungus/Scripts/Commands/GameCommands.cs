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
			Action callAction;
			
			public Call(Action _callAction)
			{
				if (_callAction == null)
				{
					Debug.LogError("Action must not be null.");
					return;
				}
				
				callAction = _callAction;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				if (callAction != null)
				{
					callAction();
				}
				
				// Execute next command
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
			Room room;
			
			public MoveToRoom(Room _room)
			{
				if (_room == null)
				{
					Debug.LogError("Room must not be null.");
					return;
				}
				
				room = _room;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
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
				// MoveToRoom always resets the command queue so no need to call onComplete
			}
		}

		/** 
		 * Switches to a different scene.
		 */
		public class MoveToScene : CommandQueue.Command
		{
			string sceneName;

			public MoveToScene(string _sceneName)
			{
				if (_sceneName == "")
				{
					Debug.LogError("Scene name must not be empty");
					return;
				}
				
				sceneName = _sceneName;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.waiting = true;
				
				// Fade out screen
				game.cameraController.Fade(0f, game.roomFadeDuration / 2f, delegate {
					Game.GetInstance().LoadScene(sceneName, true);
				});
			}
		}

		/** 
		 * Sets a globally accessible integer value
		 */
		public class SetValue : CommandQueue.Command
		{
			string key;
			int value;
			
			public SetValue(string _key, int _value)
			{
				key = _key;
				value = _value;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game.GetInstance().SetGameValue(key, value);
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}

		/** 
		 * Sets a globally accessible string value
		 */
		public class SetString : CommandQueue.Command
		{
			string key;
			string value;
			
			public SetString(string _key, string _value)
			{
				key = _key;
				value = _value;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game.stringTable.SetString(key, value);
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}

		/** 
		 * Save the current game state to persistant storage.
		 */
		public class Save : CommandQueue.Command
		{
			Action commandAction;

			public Save(string saveName)
			{
				commandAction = delegate {
					Game.GetInstance().SaveGame(saveName);
				};
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				commandAction();

				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}

		/** 
		 * Load the game state from persistant storage.
		 */
		public class Load : CommandQueue.Command
		{
			Action commandAction;
			
			public Load(string saveName)
			{
				commandAction = delegate {
					Game.GetInstance().LoadGame(saveName);
				};
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				commandAction();
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
	}
}