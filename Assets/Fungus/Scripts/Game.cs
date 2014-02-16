using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Manages movement between rooms and global game state
	[RequireComponent (typeof (CommandQueue))]
	[RequireComponent (typeof (CameraController))]
	public class Game : MonoBehaviour 
	{
		public bool showLinks = true;

		public Room activeRoom;

		[HideInInspector]
		public View activeView;

		[HideInInspector]
		public Page activePage;

		public GameState state = new GameState();

		protected Dictionary<string, string> stringTable = new Dictionary<string, string>();

		private static Game instance;

		CameraController cameraController;

		CommandQueue commandQueue;

		public float fadeDuration = 1f;

		public string continueText = "Continue";
		
		public int charactersPerSecond = 60;

		public static Game GetInstance()
		{
			if (!instance)
			{
				instance = GameObject.FindObjectOfType(typeof(Game)) as Game;
				if (!instance)
				{
					Debug.LogError("There must be one active Game object in your scene.");
				}
			}
			
			return instance;
		}

		public virtual void Start()
		{
			cameraController = GetComponent<CameraController>();
			commandQueue = GetComponent<CommandQueue>();

			if (activeRoom == null)
			{
				// Pick first room found if none is specified
				activeRoom = GameObject.FindObjectOfType(typeof(Room)) as Room;
			}

			if (activeRoom != null)
			{
				MoveToRoom(activeRoom);
			}
		}

		public Room GetCurrentRoom()
		{
			return GetInstance().activeRoom;
		}

		public void ResetCommandQueue()
		{
			commandQueue.Reset();
		}
		
		public void ExecuteCommandQueue()
		{
			commandQueue.Execute();
		}

		public void MoveToRoom(Room room)
		{
			if (room == null)
			{
				Debug.LogError("Failed to move to room. Room must not be null.");
				return;
			}

			// Fade out screen
			cameraController.Fade(0f, fadeDuration / 2f, delegate {

				if (activeRoom != null)
				{
					activeRoom.Leave();
				}

				activeRoom = room;

				activeRoom.Enter();
				
				// Fade in screen
				cameraController.Fade(1f, fadeDuration / 2f, null);
			});
		}

		public void Restart()
		{
			// TODO: Reload scene
		}
		
		public void ClearFlags()
		{
			state.ClearFlags();
		}
		
		public bool GetFlag(string key)
		{
			return state.GetFlag(key);
		}
		
		public void SetFlag(string key, bool value)
		{
			state.SetFlag(key, value);
		}
		
		public void ClearCounters()
		{
			state.ClearCounters();
		}
		
		public int GetCounter(string key)
		{
			return state.GetCounter(key);
		}
		
		public void SetCounter(string key, int value)
		{
			state.SetCounter(key, value);
		}
		
		public void ClearInventory()
		{
			state.ClearInventory();
		}
		
		public int GetInventory(string key)
		{
			return state.GetInventory(key);
		}

		public void SetInventory(string key, int value)
		{
			state.SetInventory(key, value);
		}
		
		public void ClearStringTable()
		{
			stringTable.Clear();
		}
		
		public string GetString(string key)
		{
			if (stringTable.ContainsKey(key))
			{
				return stringTable[key];
			}
			return "";
		}
		
		public void SetString(string key, string value)
		{
			stringTable[key] = value;
		}
	}
}