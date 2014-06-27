using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/** 
 * @package Fungus An open source library for Unity 3D for creating graphic interactive fiction games.
 */
namespace Fungus
{
	/** 
	 * Manages global game state and movement between rooms.
	 */
	[RequireComponent(typeof(Dialog))]
	[RequireComponent(typeof(CommandQueue))]
	[RequireComponent(typeof(CameraController))]
	[RequireComponent(typeof(PageController))]
	public class Game : GameController 
	{
		/**
		 * The currently active Room.
		 * Only one Room may be active at a time.
		 */
		public Room activeRoom;

		/**
		 * Automatically display links between connected Rooms.
		 */
		public bool showLinks = true;

		/**
		 * Deletes all previously stored variables on launch.
		 * Disable this if you want to have game variables persist between game launches.
		 * It is then up to you to provide an in-game option to reset game variables.
		 */
		public bool deleteVariables = true;

		/**
		 * Time for fade transition to complete when moving to a different Room.
		 */
		public float roomFadeDuration = 1f;

		/**
		 * Time for fade transition to complete when hiding/showing buttons.
		 */
		public float buttonFadeDuration = 0.25f;

		/**
		 * Time which must elapse before buttons will automatically hide.
		 */
		public float autoHideButtonDuration = 5f;

		/**
		 * Currently active Dialog object used to display character text and menus.
		 */
		public Dialog dialog;

		/**
		 * Loading image displayed when loading a scene using MoveToScene()
		 */
		public Texture2D loadingImage;

		[HideInInspector]
		public CommandQueue commandQueue;
		
		[HideInInspector]
		public CameraController cameraController;
	
		[HideInInspector]
		public PageController pageController;

		/**
		 * True when executing a Wait() or WaitForTap() command
		 */
		[HideInInspector]
		public bool waiting; 

		float autoHideButtonTimer;

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
			// Acquire references to required components

			commandQueue = gameObject.GetComponent<CommandQueue>();
			cameraController = gameObject.GetComponent<CameraController>();
			pageController = gameObject.GetComponent<PageController>();

			// Auto-hide buttons should be visible at start of game
			autoHideButtonTimer = autoHideButtonDuration;

			if (activeRoom == null)
			{
				// Pick first room found if none is specified
				activeRoom = GameObject.FindObjectOfType(typeof(Room)) as Room;
			}

			if (activeRoom != null)
			{
				// Move to the active room
				commandQueue.Clear();
				commandQueue.AddCommand(new Command.MoveToRoom(activeRoom));
				commandQueue.Execute();
			}

			if (deleteVariables)
			{
				Variables.DeleteAll();
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

		public IDialog GetDialog()
		{
			if (dialog != null)
			{
				return dialog as IDialog;
			}
			
			return pageController as IDialog;
		}

		/**
		 * Returns true if the game should display 'auto hide' buttons.
		 * Buttons will be displayed if the game is not currently displaying story text/options, and no Wait command is in progress.
		 */
		public bool GetShowAutoButtons()
		{
			if (waiting)
			{
				return false;
			}

			IDialog dialog = GetDialog();

			if (dialog == null ||
			    dialog.GetDialogMode() == DialogMode.Idle)
			{
				return (autoHideButtonTimer > 0f);
			}

			return false;
		}

		/**
		 * Loads a new scene and displays an optional loading image.
		 * This is useful for splitting a large Fungus game across multiple scene files to reduce peak memory usage.
		 * All previously loaded assets (including textures and audio) will be released.
		 * @param sceneName The filename of the scene to load.
		 * @param saveGame Automatically save the current game state as a checkpoint.
		 */
		public void LoadScene(string sceneName, bool saveGame)
		{
			SceneLoader.LoadScene(sceneName, loadingImage, saveGame);
		}

		/**
		 * Save the current game variables to persistant storage.
		 * Store the currently loaded scene name so that Game.LoadGame() can automatically move to the appropriate scene.
		 */
		public void SaveGame()
		{
			SetString("_scene", Application.loadedLevelName);
			Variables.Save();
		}

		/**
		 * Loads the current game state from persistant storage.
		 * This will cause the scene specified in the "_scene" string to be loaded.
		 * Each scene in your game should contain the necessary code to restore the current game state based on saved data.
		 * @param saveName The name of the saved game data.
		 */
		public void LoadGame(string saveName = "_fungus")
		{
			Variables.SetSaveName(saveName);
			string sceneName = Variables.GetString("_scene");
			if (sceneName.Length > 0)
			{
				MoveToScene(sceneName);
			}
		}
	}
}