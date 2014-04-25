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
		 * Time which must elapse before buttons will automatically hide.
		 */
		public float autoHideButtonDuration = 5f;

		/**
		 * Full screen texture used for screen fade effect.
		 */
		public Texture2D screenFadeTexture;

		/**
		 * Position of continue and swipe icons in normalized screen space coords.
		 * (0,0) = top left, (1,1) = bottom right
		 */
		public Vector2 iconPosition = new Vector2(1,1);

		/**
		 * Icon to display when waiting for player input to continue
		 */
		public Texture2D continueIcon;

		/**
		 * Icon to display when swipe pan mode is active.
		 */
		public Texture2D swipePanIcon;

		/**
		 * Sound effect to play when buttons are clicked.
		 */
		public AudioClip buttonClickClip;

		/**
		 * Default screen position for Page when player enters a Room.
		 */
		public PageController.PagePosition defaultPagePosition;

		/**
		 * Default width and height of Page as a fraction of screen height [0..1]
		 */
		public Vector2 defaultPageScale = new Vector2(0.75f, 0.25f);

		/**
		 *  Automatically center the Page when player is choosing from multiple options.
		 */
		public bool centerChooseMenu = true;

		/**
		 * Width of Page as a fraction of screen width [0..1] when automatically centering a Choose menu. 
		 * This setting only has an effect when centerChooseMenu is enabled.
		 */
		public float chooseMenuWidth = 0.5f;

		/**
		 * Global dictionary of integer values for storing game state.
		 */
		[HideInInspector]
		public Dictionary<string, int> values = new Dictionary<string, int>();

		[HideInInspector]
		public StringTable stringTable = new StringTable();
		
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

		[HideInInspector]
		public bool swipePanActive;

		[HideInInspector]
		public float fadeAlpha = 0f;

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
			// Add components for additional game functionality
			commandQueue = gameObject.AddComponent<CommandQueue>();
			cameraController = gameObject.AddComponent<CameraController>();
			pageController = gameObject.AddComponent<PageController>();

			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = false;
			audioSource.loop = true;

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

		void OnGUI()
		{	
			if (swipePanActive)
			{
				// Draw the swipe panning icon
				if (swipePanIcon)
				{
					float x = Screen.width * iconPosition.x;
					float y = Screen.height * iconPosition.y;
					float width = swipePanIcon.width;
					float height = swipePanIcon.height;

					x = Mathf.Max(x, 0);
					y = Mathf.Max(y, 0);
					x = Mathf.Min(x, Screen.width - width);
					y = Mathf.Min(y, Screen.height - height);

					Rect rect = new Rect(x, y, width, height);
					GUI.DrawTexture(rect, swipePanIcon);
				}
			}

			if (pageController.mode == PageController.Mode.Say &&
			    pageController.FinishedWriting())
			{
				// Draw the continue icon
				if (continueIcon)
				{
					float x = Screen.width * iconPosition.x;
					float y = Screen.height * iconPosition.y;
					float width = continueIcon.width;
					float height = continueIcon.height;
					
					x = Mathf.Max(x, 0);
					y = Mathf.Max(y, 0);
					x = Mathf.Min(x, Screen.width - width);
					y = Mathf.Min(y, Screen.height - height);
					
					Rect rect = new Rect(x, y, width, height);
					GUI.DrawTexture(rect, continueIcon);
				}
			}

			// Draw full screen fade texture
			if (fadeAlpha < 1f)
			{
				// 1 = scene fully visible
				// 0 = scene fully obscured
				GUI.color = new Color(1,1,1, 1f - fadeAlpha);	
				GUI.depth = -1000;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), screenFadeTexture);
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
		 * Buttons will be displayed if the game is not currently displaying story text/options, and no Wait command is in progress.
		 */
		public bool GetShowAutoButtons()
		{
			if (waiting)
			{
				return false;
			}

			if (pageController == null ||
			    pageController.mode == PageController.Mode.Idle)
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
		 * @return The integer value for the specified key, or 0 if the key does not exist.
		 */
		public int GetGameValue(string key)
		{
			if (values.ContainsKey(key))
			{
				return values[key];
			}
			return 0;
		}

		/**
		 * Returns a parallax offset vector based on the camera position relative to the active Room.
		 * Higher values for the parallaxFactor yield a larger offset (appears closer to camera).
		 * Suggested range for good parallax effect is [0.1..0.5].
		 * @param parallaxFactor Horizontal and vertical scale factors to apply to camera offset vector.
		 * @return A parallax offset vector based on camera positon relative to current room and the parallaxFactor.
		 */
		public Vector3 GetParallaxOffset(Vector2 parallaxFactor)
		{
			if (activeRoom == null)
			{
				return Vector3.zero;
			}

			Vector3 a = activeRoom.transform.position;
			Vector3 b = cameraController.GetCameraPosition();
			Vector3 offset = (a - b);
			offset.x *= parallaxFactor.x;
			offset.y *= parallaxFactor.y;

			return offset;
		}
	}
}