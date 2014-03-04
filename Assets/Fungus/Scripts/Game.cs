using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * @mainpage notitle
 * This is the code documentation for Fungus, a Unity 3D plugin created by Chris Gregan of Snozbot.
 * 
 * @note For a list of all supported scripting commands, please see the Fungus.GameController class documentation.
 * 
 * Refer to http://www.snozbot.com/fungus for more information about Fungus.
 */

/** 
 * @package Fungus An open source library for Unity 3D for creating graphic interactive fiction games.
 */
namespace Fungus
{
	/** 
	 * Manages global game state and movement between rooms.
	 */
	public class Game : MonoBehaviour 
	{
		/**
		 * The currently active Room.
		 * Only one Room may be active at a time.
		 */
		public Room activeRoom;

		/**
		 * The style to apply when displaying Pages.
		 */
		public PageStyle activePageStyle;

		/**
		 * Automatically display links between connected Rooms.
		 */
		public bool showLinks = true;

		/**
		 * Text to use on 'Continue' buttons
		 */
		public string continueText = "Continue";

		/**
		 * Writing speed for page text.
		 */
		public int charactersPerSecond = 60;

		/**
		 * Fixed Z coordinate of main camera.
		 */
		public float cameraZ = - 10f;

		/**
		 * Time for transition to complete when moving to a different Room.
		 */
		public float roomFadeDuration = 1f;

		/**
		 * Full screen texture used for screen fade effect
		 */
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

			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;

			// Create a default page style if none exists


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

		/**
		 * Moves player to a different room.
		 */
		public void MoveToRoom(Room room)
		{
			if (room == null)
			{
				Debug.LogError("Failed to move to room. Room must not be null.");
				return;
			}

			// Fade out screen
			cameraController.Fade(0f, roomFadeDuration / 2f, delegate {

				activeRoom = room;

				// Notify room script that the Room is being entered
				// Calling private method on Room to hide implementation
				activeRoom.gameObject.SendMessage("Enter");
				
				// Fade in screen
				cameraController.Fade(1f, roomFadeDuration / 2f, null);
			});
		}
	}
}