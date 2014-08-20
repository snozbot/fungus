using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Fungus.Script;

/** 
 * @package Fungus An open source library for Unity 3D for creating graphic interactive fiction games.
 */
namespace Fungus
{
	/** 
	 * Manages global game state and movement between rooms.
	 */
	[RequireComponent(typeof(Dialog))]
	public class Game : MonoBehaviour 
	{
		/**
		 * Fade transition time when hiding/showing buttons.
		 */
		[Range(0,5)]
		[Tooltip("Fade transition time when hiding/showing buttons.")]
		public float buttonFadeDuration = 0.25f;

		/**
		 * Time to elapse before buttons hide automatically.
		 */
		[Range(0, 60)]
		[Tooltip("Time to elapse before buttons hide automatically.")]
		public float autoHideButtonDuration = 5f;

		/**
		 * Currently active Dialog object used to display character text and menus.
		 */
		[Tooltip("Currently active Dialog object used to display character text and menus.")]
		public Dialog dialog;

		/**
		 * Loading image displayed when loading a scene using MoveToScene() command.
		 */
		[Tooltip("Loading image displayed when loading a scene using MoveToScene() command.")]
		public Texture2D loadingImage;

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
			// Auto-hide buttons should be visible at start of game
			autoHideButtonTimer = autoHideButtonDuration;
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
			
			return null;
		}

		/**
		 * Returns true if the game should display 'auto hide' buttons.
		 * Buttons will be displayed if the game is not currently displaying story text/options, and no Wait command is in progress.
		 */
		public bool GetShowAutoButtons()
		{
			CameraController cameraController = CameraController.GetInstance();

			if (cameraController.waiting)
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
		 * Save the current game variables to persistant storage using a save game name.
		 * Stores the currently loaded scene name so that Game.LoadGame() can automatically move to the appropriate scene.
		 */
		public void SaveGame(string saveName = "_fungus")
		{
			GlobalVariables.SetString("_scene", Application.loadedLevelName);
			GlobalVariables.Save(saveName);
		}

		/**
		 * Loads the current game state from persistant storage using a save game name.
		 * This will cause the scene specified in the "_scene" string to be loaded.
		 * Each scene in your game should contain the necessary code to restore the current game state based on saved data.
		 * @param saveName The name of the saved game data.
		 */
		public void LoadGame(string saveName = "_fungus")
		{
			GlobalVariables.Load(saveName);
			string sceneName = GlobalVariables.GetString("_scene");
			if (sceneName.Length > 0)
			{
			//	MoveToScene(sceneName);
			}
		}
	}
}