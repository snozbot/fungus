using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Manages global game state and movement between rooms
	public class Game : MonoBehaviour 
	{
		public Room activeRoom;

		public bool showLinks = true;

		public string continueText = "Continue";
		
		public int charactersPerSecond = 60;

		// Fixed Z coordinate of camera
		public float cameraZ = - 10f;

		public float fadeDuration = 1f;

		public Texture2D fadeTexture;

		[HideInInspector]
		public View activeView;
		
		[HideInInspector]
		public Page activePage;
		
		[HideInInspector]
		public GameState state = new GameState();
		
		[HideInInspector]
		public StringTable stringTable = new StringTable();
		
		[HideInInspector]
		public CommandQueue commandQueue;
		
		[HideInInspector]
		public CameraController cameraController;

		static Game instance;

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
			commandQueue = gameObject.AddComponent<CommandQueue>();
			cameraController = gameObject.AddComponent<CameraController>();

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

		public void MoveToRoom(Room room)
		{
			if (room == null)
			{
				Debug.LogError("Failed to move to room. Room must not be null.");
				return;
			}

			// Fade out screen
			cameraController.Fade(0f, fadeDuration / 2f, delegate {

				activeRoom = room;

				// Notify room script that the Room is being entered
				// Calling private method on Room to hide implementation
				activeRoom.gameObject.SendMessage("Enter");
				
				// Fade in screen
				cameraController.Fade(1f, fadeDuration / 2f, null);
			});
		}
	}
}