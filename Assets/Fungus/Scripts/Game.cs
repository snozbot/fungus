using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/** 
 * @package Fungus An open source library for Unity 3D for creating graphic interactive fiction games.
 */
namespace Fungus
{
	/** 
	 * Manages global game state and movement between rooms.
	 */
	public class Game : GameController 
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
		 * Writing speed for page text.
		 */
		public int charactersPerSecond = 60;

		/**
		 * Fixed Z coordinate of main camera.
		 */
		public float cameraZ = - 10f;

		/**
		 * Time for fade transition to complete when moving to a different Room.
		 */
		public float roomFadeDuration = 1f;

		/**
		 * Time for fade transition to complete when hiding/showing buttons.
		 */
		public float buttonFadeDuration = 0.25f;

		/**
		 * Full screen texture used for screen fade effect.
		 */
		public Texture2D fadeTexture;

		/**
		 * Sound effect to play when buttons are clicked.
		 */
		public AudioClip buttonClickClip;

		/**
		 * Time which must elapse before buttons will automatically hide.
		 */
		public float autoHideButtonDuration = 5f;

		/**
		 * References to a style object which controls the appearance & behavior of the continue button.
		 */
		public ContinueStyle continueStyle;

		float autoHideButtonTimer;

		/**
		 * Global dictionary of integer values for storing game state.
		 */
		[HideInInspector]
		public Dictionary<string, int> values = new Dictionary<string, int>();

		[HideInInspector]
		public View activeView;
		
		[HideInInspector]
		public Page activePage;

		[HideInInspector]
		public StringTable stringTable = new StringTable();
		
		[HideInInspector]
		public CommandQueue commandQueue;
		
		[HideInInspector]
		public CameraController cameraController;

		/**
		 * True when executing a Wait() or WaitForTap() command
		 */
		[HideInInspector]
		public bool waiting; 

		static Game instance;

		/**
		 * Returns the singleton instance for the Game class
		 */
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
			// Add components for additional game functionality
			commandQueue = gameObject.AddComponent<CommandQueue>();
			cameraController = gameObject.AddComponent<CameraController>();

			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;

			if (activeRoom == null)
			{
				// Pick first room found if none is specified
				activeRoom = GameObject.FindObjectOfType(typeof(Room)) as Room;
			}

			if (activeRoom != null)
			{
				// Move to the active room
				commandQueue.Clear();
				commandQueue.AddCommand(new Command.MoveToRoomCommand(activeRoom));
				commandQueue.Execute();
			}
		}

		public virtual void Update()
		{
			autoHideButtonTimer -= Time.deltaTime;
			autoHideButtonTimer = Mathf.Max(autoHideButtonTimer, 0f);

			if (Input.GetMouseButtonDown(0))
			{
				autoHideButtonTimer = autoHideButtonDuration;
			}
		}

		/**
		 * Plays the button clicked sound effect
		 */
		public void PlayButtonClick()
		{
			if (buttonClickClip == null)
			{
				return;
			}
			audio.PlayOneShot(buttonClickClip);
		}

		/**
		 * Returns true if the game should display 'auto hide' buttons.
		 * Buttons will be displayed if the active page is not currently displaying story text/options, and no Wait command is in progress.
		 */
		public bool ShowAutoButtons()
		{
			if (waiting)
			{
				return false;
			}

			if (activePage == null ||
			    activePage.mode == Page.Mode.Idle)
			{
				return (autoHideButtonTimer > 0f);
			}

			return false;
		}

		/**
		 * Sets a globally accessible game state value.
		 * @param key The key of the value.
		 * @param value The integer value to store.
		 */
		public void SetGameValue(string key, int value)
		{
			values[key] = value;
		}

		/**
		 * Gets a globally accessible game state value.
		 * @param key The key of the value.
		 * @return value The integer value for the specified key, or 0 if the key does not exist.
		 */
		public int GetGameValue(string key)
		{
			if (values.ContainsKey(key))
			{
				return values[key];
			}
			return 0;
		}
	}
}