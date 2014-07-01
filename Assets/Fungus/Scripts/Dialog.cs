using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/**
	 * Permitted states for Dialogs.
     */
	public enum DialogMode
	{
		/// Dialog has no pending content to display so is not shown.
		Idle,
		/// Dialog is currently writing out content.
		Writing,
		/// Dialog has finished writing out content and is waiting for player input.
		Waiting
	}
	
	/**
	 * Interface for Dialog implementations.
	 * This allows us to introduce new types of Dialog in future.
	 */
	public interface IDialog
	{
		/**
		 * Returns the current state of the Dialog.
		 */
		DialogMode GetDialogMode();

		/**
		 * Display a line of story text.
		 * If any options have previously been added using AddOption(), these will be displayed and the
		 * user must choose an option to continue. The sayAction parameter is ignored 
		 * @param sayText The line of story text to be displayed.
		 * @param sayAction Delegate method to call when the player taps to continue.
		 */
		void Say(string sayText, Action sayAction);

		/**
		 * Clear the current list of options previously added using AddOption().
		 */
		void ClearOptions();

		/**
		 * Add an option to the list of options to be displayed on the next call to Say().
		 * @param optionText Text to display on the button for the option.
		 * @param optionAction Delegate method to call when the option button is pressed.
		 */
		void AddOption(string optionText, Action optionAction);

		/**
		 * Sets a time limit for the player to choose between multiple options.
		 * If the player fails to make a choice in time then the timeoutAction delegate method is called.
		 * This setting only applies to the next Say() command and will be reset afterwards.
		 * @timeoutDuration Length of time for the player to choose an option.
		 * @timeoutAction Delegate method to call when player fails to choose an option.
		 */
		void SetTimeout(float timeoutDuration, Action timeoutAction);
	}

	[ExecuteInEditMode]
	public class Dialog : MonoBehaviour, IDialog
	{
		/**
		 * Padding values for each side of the dialog.
		 * Values are in normalized screen coords [0..1]
		 */
		[Serializable]
		public class Layout
		{
			/**
			 * Push the left dialog edge away from the left side of the screen.
			 */
			[Tooltip("Push the left dialog edge away from the left side of the screen.")]
			public bool leftSpace = false;

			/**
			 * Push the top dialog edge away from the top side of the screen.
			 */
			[Tooltip("Push the top dialog edge away from the top side of the screen.")]
			public bool topSpace = true;

			/**
			 * Push the right dialog edge away from the right side of the screen.
			 */
			[Tooltip("Push the right dialog edge away from the right side of the screen.")]
			public bool rightSpace = false;

			/**
			 * Push the bottom dialog edge away from the bottom side of the screen.
			 */
			[Tooltip("Push the bottom dialog edge away from the bottom side of the screen.")]
			public bool bottomSpace = false;

			/**
			 * Minimum dimensions of the dialog in normalized screen coordinates [0..1].
			 * The dialog may expand beyond these dimensions to fit content.
			 */
			[Tooltip("Minimum dimensions of the dialog in normalized screen coordinates [0..1].")]
			public Vector2 size = new Vector2(1f, 0.25f);

			/**
			 * Offset the dialog relative to the top left corner of the screen.
			 * Coordinates are in normalized screen coordinates [0..1]
			 */
			[Tooltip("Offset the dialog relative to the top left corner of the screen.")]
			public Vector2 offset;
		}
		
		/**
		 * Layout values used to control size and position of the dialog.
		 */
		[Tooltip("Layout values used to control size and position of the dialog.")]
		public Layout layout;

		/**
		 * Defines the dialog display properties of a game character.
		 */
		[System.Serializable]
		public class Character
		{
			/**
		 	 * Side of screen to display character image.
		 	 */
			public enum ImageSide
			{
				/// Display image on the left side of the dialog.
				Left,
				/// Display image on the right side of the dialog.
				Right
			}

			/**
			 * Identifier for this character for use with the SetCharacter() command.
			 */
			[Tooltip("Identifier for this character for use with the SetCharacter() command.")]
			public string characterID;

			/**
			 * Name text to display for the character on the dialog.
			 * If empty then the name field is not displayed.
			 */
			[Tooltip("Name text to display for the character on the dialog.")]
			public string name;

			/**
			 * The color of the name text label.
			 * This always overrides any color value specified in the NameStyle property.
			 */
			[Tooltip("The color of the name text label.")]
			public Color nameColor;

			/**
			 * Image to display for the character.
			 * If no image is specified then no character image will be displayed.
			 */
			[Tooltip("Image to display for the character.")]
			public Texture2D image;
			
			/**
		 	 * Side of dialog where character image will be displayed.
		 	 */
			[Tooltip("Side of dialog where character image will be displayed.")]
			public ImageSide imageSide;
		}
		
		/**
		 * List of game characters that can be displayed using this dialog.
		 */
		[Tooltip("List of game characters that can be displayed using this dialog.")]
		public Character[] characters;

		/**
		 * Active character to use when displaying dialog (set using the SetCharacter() command).
		 */
		[Tooltip("Active character to use when displaying dialog.")]
		public int activeCharacter;

		/**
		 * Writing speed for say text in characters per second.
		 */
		[Range(0, 1000)]
		[Tooltip("Writing speed for say text in characters per second.")]
		public int writingSpeed = 100;

		/**
		 * Sound to play while text is being written in the dialog.
		 */
		[Tooltip("Sound to play while text is being written in the dialog.")]
		public AudioClip writingSound;

		/**
		 * Loop the writing sound as long as text is being written.
		 */
		[Tooltip("Loop the writing sound while text is being written.")]
		public bool loopWritingSound = true; 
		
		/**
		 * Sound effect to play when the player taps to continue.
		 */
		[Tooltip("Sound effect to play when the player taps to continue.")]
		public AudioClip continueSound;

		/**
		 * Icon to display under the story text when waiting for player to tap to continue.
		 */
		[Tooltip("Icon to display under the story text when waiting for player to tap to continue.")]
		public Texture2D continueIcon;

		/**
		 * Number of buttons to display in the same row when showing multiple options.
		 */
		[Range(0, 10)]
		[Tooltip("Number of buttons to display in the same row when showing multiple options.")]
		public int buttonsPerRow = 2;

		/**
		 * Minimum width of each button as a fraction of the screen width [0..1].
		 * This is useful to create a column of buttons with matching width.
		 */
		[Range(0, 1)]
		[Tooltip("Minimum width of each button as a fraction of the screen width [0..1].")]
		public float minButtonWidth = 0;

		/**
		 * Padding values for each side of the character image.
		 */
		[System.Serializable]
		public class ImagePadding
		{
			/**
			 * Padding to apply to left side of image as a fraction of screen height [-2..2].
			 */
			[Range(-2,2)]
			[Tooltip("Padding to apply to left side of image as a fraction of screen height [-2..2].")]
			public float left;

			/**
			 * Padding to apply to right side of image as a fraction of screen height [-2..2].
			 */
			[Range(-2,2)]
			[Tooltip("Padding to apply to right side of image as a fraction of screen height [-2..2].")]
			public float right;

			/**
			 * Padding to apply to top side of image as a fraction of screen height [-1..1].
			 */
			[Range(-1,1)]
			[Tooltip("Padding to apply to top side of image as a fraction of screen height [-1..1].")]
			public float top;

			/**
			 * Padding to apply to bottom side of image as a fraction of screen height [-1..1].
			 */
			[Range(-1,1)]
			[Tooltip("Padding to apply to bottom side of image as a fraction of screen height [-1..1].")]
			public float bottom;
		}

		/**
		 * Padding offset to apply around the character image.
		 */
		[Tooltip("Padding offset to apply around the character image.")]
		public ImagePadding imagePadding;

		/**
		 * Scale of character image, specified as a fraction of current screen height [0..1].
		 */
		[Range(0, 1)]
		[Tooltip("Scale of character image, specified as a fraction of current screen height [0..1].")]
		public float imageScale = 0.25f;

		/**
		 * Animation frames for multiple choice time indicator.
		 */
		[Tooltip("Animation frames for multiple choice time indicator.")]
		public Texture2D[] timerAnimation;

		/**
		 * Scale of timer image, specified as a fraction of current screen height [0..1].
		 */
		[Range(0, 1)]
		[Tooltip("Scale of timer image, specified as a fraction of current screen height [0..1].")]
		public float timerScale = 0.2f;

		/**
		 * Sound effect to play when time indicator advances.
		 */
		[Tooltip("Sound effect to play when time indicator advances.")]
		public AudioClip timerSound;

		/**
		 * Style for dialog box background.
		 */
		[Tooltip("Style for dialog box background.")]
		public GUIStyle boxStyle;
		
		/**
		 * Style for name text.
		 */
		[Tooltip("Style for name text.")]
		public GUIStyle nameTextStyle;

		/**
		 * Style for say text.
		 */
		[Tooltip("Style for say text.")]
		public GUIStyle sayTextStyle;

		/**
		 * Style for option buttons
		 */
		[Tooltip("Style for option buttons.")]
		public GUIStyle buttonStyle;

		DialogMode dialogMode;

		public DialogMode GetDialogMode()
		{
			return dialogMode;
		}

		class Option
		{
			public string optionText;
			public Action optionAction;
			
			public Option(string _optionText, Action _optionAction)
			{
				optionText = _optionText;
				optionAction = _optionAction;
			}
		}

		List<Option> options = new List<Option>();

		float timeoutDuration;
		Action timeoutAction;
		float timeoutTimer;
		int timeoutAnimationIndex;

		string sayText = "";
		string previousSayText = "";
		string displayedSayText = "";
		
		Action deferredAction;
		Action continueAction;
		bool executeAsCommand;

		float continueTimer;
		bool instantCompleteText;
		bool fullscreen;

		// Cache scaled GUIStyle objects so we're not creating lots of new objects every frame
		GUIStyle cachedBoxStyle;
		GUIStyle cachedNameTextStyle;
		GUIStyle cachedSayTextStyle;
		GUIStyle cachedButtonStyle;

		void Start()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			CacheScaledStyles();

			fullscreen = Screen.fullScreen;
		}

		void Update()
		{
			writingSpeed = Math.Max(writingSpeed, 0);
			buttonsPerRow = Math.Max(buttonsPerRow, 1);
			minButtonWidth = Mathf.Max(minButtonWidth, 0);
			imageScale = Mathf.Max(imageScale, 0);
			timerScale = Mathf.Max(timerScale, 0);
			layout.size.x = Mathf.Clamp01(layout.size.x);
			layout.size.y = Mathf.Clamp01(layout.size.y);
			layout.offset.x = Mathf.Clamp01(layout.offset.x);
			layout.offset.y = Mathf.Clamp01(layout.offset.y);

			if (!Application.isPlaying)
			{
				return;
			}

			if (continueTimer > 0)
			{
				continueTimer -= Time.deltaTime;
				continueTimer = Mathf.Max(continueTimer, 0f);
			}

			if (sayText.Length == 0)
			{
				dialogMode = DialogMode.Idle;
			}

			// Check if the sayText field has been modified directly.
			// If that has happened, then write the text with no Action
			if (sayText != previousSayText &&
				sayText.Length > 0)
			{
				StopAllCoroutines();
				Say(sayText, null);
			}

			if (timeoutTimer > 0)
			{
				timeoutTimer -= Time.deltaTime;
				timeoutTimer = Mathf.Max(timeoutTimer, 0f);
			}

			// Update cached GUIStyles when running in editor or when switching to/from fullscreen
			if (Application.isEditor || 
			    fullscreen != Screen.fullScreen)
			{
				CacheScaledStyles();

				fullscreen = Screen.fullScreen;
			}
		}

		void CacheScaledStyles()
		{
			cachedBoxStyle = ScaleFontSize(boxStyle);
			cachedNameTextStyle = ScaleFontSize(nameTextStyle);
			cachedSayTextStyle = ScaleFontSize(sayTextStyle);
			cachedButtonStyle = ScaleFontSize(buttonStyle);
		}

		public void Say(string _sayText, Action sayAction)
		{
			string copyText = _sayText;

			// Hack: Handle Say(""); by writing a single space character.
			if (copyText.Length == 0)
			{
				copyText = " ";
			}

			continueAction = sayAction;
			previousSayText = copyText;

			if (timeoutDuration > 0 &&
				options.Count > 0)
			{
				// Activate timeout timer
				timeoutTimer = timeoutDuration;
			}

			WriteSayText(copyText);
		}

		void WriteSayText(string _sayText)
		{
			// Disable quick continue for a short period to prevent accidental taps
			continueTimer = 0.4f;
			instantCompleteText = false;

			sayText = _sayText;
			
			if (_sayText.Contains("<"))
			{
				// Hack to avoid displaying partial color tag text - write instantly
				displayedSayText = _sayText;
				dialogMode = DialogMode.Waiting;
			}
			else
			{
				// Use a coroutine to write the story text out over time
				StartCoroutine(WriteStoryInternal());
			}
		}
		
		// Coroutine to write story text out over a period of time
		IEnumerator WriteStoryInternal()
		{
			// Zero CPS means write instantly
			if (writingSpeed == 0)
			{
				displayedSayText = sayText;
				yield break;
			}

			GameObject typingAudio = null;

			if (writingSound != null)
			{
				typingAudio = new GameObject("WritingSound");
				typingAudio.AddComponent<AudioSource>();
				typingAudio.audio.clip = writingSound;
				typingAudio.audio.loop = loopWritingSound;
				typingAudio.audio.Play();
			}

			dialogMode = DialogMode.Writing;

			displayedSayText = "";
			
			// Make one character visible at a time
			float writeDelay = (1f / (float)writingSpeed);
			float timeAccumulator = 0f;
			int i = 0;
			
			while (true)
			{
				if (instantCompleteText)
				{
					instantCompleteText = false;
					displayedSayText = sayText;
					break;
				}

				timeAccumulator += Time.deltaTime;
				
				while (timeAccumulator > writeDelay)
				{
					i++;
					timeAccumulator -= writeDelay;
				}
				
				if (i >= sayText.Length)
				{
					displayedSayText = sayText;
					break;
				}
				else
				{
					string left = sayText.Substring(0, i + 1);
					string right = sayText.Substring(i + 1);
					
					displayedSayText = left;
					displayedSayText += "<color=#FFFFFF00>";
					displayedSayText += right;
					displayedSayText += "</color>";
				}
				
				yield return null;
			}

			dialogMode = DialogMode.Waiting;

			if (typingAudio != null)
			{
				Destroy(typingAudio);
			}
		}

		GUIStyle ScaleFontSize(GUIStyle style)
		{
			GUIStyle styleCopy = new GUIStyle(style);
			styleCopy.fontSize = Mathf.RoundToInt(styleCopy.fontSize * ((float)Screen.height / 768f));
			return styleCopy;
		}

		public void ClearOptions()
		{
			options.Clear();
		}
		
		public void AddOption(string optionText, Action optionAction)
		{
			options.Add(new Option(optionText, optionAction));
		}

		public void SetTimeout(float _timeoutDuration, Action _timeoutAction)
		{
			timeoutDuration = _timeoutDuration;
			timeoutAction = _timeoutAction;
		}

		public void SetCharacter(string characterID)
		{
			for (int i = 0; i < characters.Length; ++i)
			{
				if (characters[i].characterID == characterID)
				{
					activeCharacter = i;
					return;
				}
			}
			Debug.Log ("Failed to find character ID " + characterID);
		}

		public virtual void OnGUI()
		{
			if (!Application.isPlaying ||
				dialogMode == DialogMode.Idle ||
			    characters.Length == 0 ||
			    activeCharacter >= characters.Length)
			{
				return;
			}
		
			Rect areaRect = new Rect(layout.offset.x * Screen.width, layout.offset.y * Screen.height, Screen.width, Screen.height);
			Rect sideImageRect = new Rect();
			Rect dialogRect = new Rect();

			// The left and right padding values are also calculated based on Screen.height to give
			// consistent padding regardless of the screen aspect ratio
			RectOffset imagePaddingRect = new RectOffset(Mathf.RoundToInt(imagePadding.left * Screen.height),
			                                             Mathf.RoundToInt(imagePadding.right * Screen.height),
			                                             Mathf.RoundToInt(imagePadding.top * Screen.height),
			                                             Mathf.RoundToInt(imagePadding.bottom * Screen.height));

			Character character = characters[activeCharacter];

			GUILayout.BeginArea(areaRect);
			{
				if (layout.topSpace)
				{
					GUILayout.FlexibleSpace();
				}

				GUILayout.BeginHorizontal();
				{
					if (layout.leftSpace)
					{
						GUILayout.FlexibleSpace();
					}

					GUILayout.BeginHorizontal(cachedBoxStyle, GUILayout.MinWidth(Screen.width * layout.size.x), GUILayout.MinHeight(Screen.height * layout.size.y));
					{
						if (character.imageSide == Character.ImageSide.Left &&
						    character.image != null)
						{
							// Reserve a rect for the side image based on the current screen height and imageScale
							float sideImageHeight = Screen.height * imageScale;
							float sideImageWidth = (sideImageHeight / character.image.height) * character.image.width;
							float w = sideImageWidth + imagePaddingRect.left + imagePaddingRect.right;
							float h = sideImageHeight + imagePaddingRect.top + imagePaddingRect.bottom;
							sideImageRect = GUILayoutUtility.GetRect(w, h, GUILayout.Width(w), GUILayout.Height(h));
						}
						else if (character.imageSide == Character.ImageSide.Right)
						{
							DrawTimer();
						}

						GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
						{
							if (character.name.Length > 0)
							{
								cachedNameTextStyle.normal.textColor = character.nameColor;
								GUILayout.Label(new GUIContent(character.name), cachedNameTextStyle);
							}

							GUILayout.FlexibleSpace();

							GUILayout.Label(new GUIContent(displayedSayText), cachedSayTextStyle);

							GUILayout.FlexibleSpace();

							// Show buttons for player options, or the continue icon if there are no options
							if (options.Count > 0)
							{	
								GUILayout.BeginHorizontal();
								{
									GUILayout.BeginVertical();
									{
										int buttonCount = 0;
										for (int i = 0; i < options.Count; ++i)
										{
											if (buttonCount == 0)
											{
												GUILayout.BeginHorizontal();
												GUILayout.FlexibleSpace();
											}
											buttonCount++;

											if (GUILayout.Button(options[i].optionText, cachedButtonStyle, GUILayout.MinWidth(Screen.width * minButtonWidth)))
											{
												AudioSource.PlayClipAtPoint(continueSound, new Vector3());

												if (options[i].optionAction == null)
												{
													deferredAction = DoNullAction;
													executeAsCommand = false;
												}
												else
												{
													deferredAction = options[i].optionAction;
													executeAsCommand = true;
												}
											}

											if (buttonCount == buttonsPerRow ||
											    i == options.Count - 1)
											{
												GUILayout.FlexibleSpace();
												GUILayout.EndHorizontal();
												buttonCount = 0;
											}
										}
									}
									GUILayout.EndVertical();
								}
								GUILayout.EndHorizontal();
							}
							else if (continueIcon != null)
							{
								GUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();

								Rect continueButtonRect = GUILayoutUtility.GetRect(continueIcon.width, 
								                                                   continueIcon.height, 
								                                                   GUILayout.MaxWidth(continueIcon.width),
								                                                   GUILayout.MaxHeight(continueIcon.height));


								if (dialogMode == DialogMode.Waiting)
								{
									continueButtonRect.y += Mathf.Sin(Time.timeSinceLevelLoad * 15f) * (float)(continueIcon.height * 0.2f);
									GUI.DrawTexture(continueButtonRect, continueIcon);
								}

								GUILayout.FlexibleSpace();
								GUILayout.EndHorizontal();
							}

							GUILayout.FlexibleSpace();
						}
						GUILayout.EndVertical();

						if (character.imageSide == Character.ImageSide.Right &&
						    character.image != null)
						{
							// Reserve a rect for the side image based on the current screen height and imageScale
							float sideImageHeight = Screen.height * imageScale;
							float sideImageWidth = (sideImageHeight / character.image.height) * character.image.width;
							float w = sideImageWidth + imagePaddingRect.left + imagePaddingRect.right;
							float h = sideImageHeight + imagePaddingRect.top + imagePaddingRect.bottom;
							sideImageRect = GUILayoutUtility.GetRect(w, h, GUILayout.Width(w), GUILayout.Height(h));
						}
						else if (character.imageSide == Character.ImageSide.Left)
						{
							DrawTimer();
						}
					}
					GUILayout.EndHorizontal();

					// Get rect of dialog for testing mouse hit later on
					dialogRect = GUILayoutUtility.GetLastRect();

					if (layout.rightSpace)
					{
						GUILayout.FlexibleSpace();
					}
				}
				GUILayout.EndHorizontal();

				if (layout.bottomSpace)
				{
					GUILayout.FlexibleSpace();
				}
			}
			GUILayout.EndArea();

			if (character.image != null)
			{
				// Adjust side image rect based on aspect ratio of the image.
				float sideImageHeight = character.image.height * ((sideImageRect.width - imagePaddingRect.left - imagePaddingRect.right) / character.image.width);
				sideImageRect.yMax = sideImageRect.yMin + sideImageHeight + imagePaddingRect.bottom + imagePaddingRect.top;
				sideImageRect = imagePaddingRect.Remove(sideImageRect);

				// Adjust rect based on layout offset
				sideImageRect.x += areaRect.x;
				sideImageRect.y += areaRect.y;

				// Draw side image
				GUI.DrawTexture(sideImageRect, character.image);
			}

			bool clickedOnDialog = (Input.GetMouseButtonUp(0) && dialogRect.Contains(Event.current.mousePosition));

			if (dialogMode == DialogMode.Writing)
			{
				if (clickedOnDialog || Input.GetKeyUp("space"))
				{
					instantCompleteText = true;
				}
			}

			if (dialogMode == DialogMode.Waiting)
			{
				// Player can continue by clicking anywhere
				if (options.Count == 0 &&
				    continueTimer == 0 &&
				    (clickedOnDialog || Input.GetKeyUp("space")))
				{
					if (continueSound != null)
					{
						AudioSource.PlayClipAtPoint(continueSound, new Vector3());
					}

					if (continueAction == null)
					{
						deferredAction = DoNullAction;
						executeAsCommand = false;
					}
					else
					{
						deferredAction = continueAction;
						executeAsCommand = false;
					}
				}

				// Check if timeout timer has expired
				if (options.Count > 0 &&
				    timeoutDuration > 0 &&
				    timeoutTimer == 0)
				{
					deferredAction = timeoutAction;
					executeAsCommand = true;
				}
			}
			
			// Execute any deferred actions on the last call to OnGUI
			if (Event.current.type == EventType.Repaint)
			{
				if (deferredAction != null)
				{
					Action tempAction = deferredAction;
					
					displayedSayText = "";
					sayText = "";
					deferredAction = null;
					dialogMode = DialogMode.Idle;
					ClearOptions();
					timeoutAction = null;
					timeoutDuration = 0;
					timeoutTimer = 0;
					timeoutAnimationIndex = 0;

					if (executeAsCommand)
					{
						executeAsCommand = false;

						// Execute next command
						CommandQueue commandQueue = Game.GetInstance().commandQueue;		
						commandQueue.CallCommandMethod(tempAction);
					}
					else
					{
						tempAction();
					}
				}
			}
		}

		void DrawTimer()
		{
			if (timeoutTimer > 0 &&
			    timerAnimation.Length > 0)
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.FlexibleSpace();

					GUILayout.BeginVertical();
					{
						GUILayout.FlexibleSpace();

						float t = 1f - timeoutTimer / timeoutDuration;
						int index = Mathf.RoundToInt(Mathf.Lerp(0, timerAnimation.Length - 1, t));
						
						// Play a sound effect for each animation frame
						if (timeoutAnimationIndex != index &&
						    timerSound != null)
						{
							AudioSource.PlayClipAtPoint(timerSound, Vector3.zero);
						}
						
						timeoutAnimationIndex = index;

						float height = Screen.height * timerScale;
						GUILayout.Box(timerAnimation[timeoutAnimationIndex], new GUIStyle(), GUILayout.Height(height));

						GUILayout.FlexibleSpace();
					}
					GUILayout.EndVertical();

					GUILayout.FlexibleSpace();
				}
				GUILayout.EndHorizontal();
			}
		}

		void DoNullAction()
		{}
	}
}