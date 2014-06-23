using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * Manages and draws a text box for rendering story text and multiple choice menus.
	 */
	[ExecuteInEditMode]
	public class PageController : MonoBehaviour, IDialog
	{
		/// Options for default Page position on screen 
		public enum PagePosition
		{
			/// Page appears full-size and horizontally centered at top of screen.
			Top,
			/// Page appears centered in middle of screen, with height fitted to content.
			Middle,
			/// Page appears full-size and horizontally centered at bottom of screen.
			Bottom
		}

		/**
		 * Defines a rect in normalized screen space coordinates.
		 * e.g. x1 = 0 means left of screen, x2 = 1 means right of screen.
		 */
		public class ScreenRect
		{
			public float x1;
			public float y1;
			public float x2;
			public float y2;
		}

		/// Options for controlling page layout 
		public enum Layout
		{
			/// Use the full rect to display the page.
			FullSize,
			/// Resize to fit displayed text and snap to top of rect.
			FitToTop,
			/// Resize to fit displayed text and snap to middle of rect.
			FitToMiddle,
			/// Resize to fit displayed text and snap to bottom of rect.
			FitToBottom
		}

		/// Controls layout of content within Page rect.
		public Layout layout = Layout.FullSize;

		/// Supported states for Page
		public enum Mode
		{
			/// No content to be displayed.
			Idle,
			/// Show a single line of text and wait for player input.
			Say,
			/// Show a multiple choice menu and wait for player to select an option.
			Choose
		};

		/**
		 * The style to apply when displaying Pages.
		 */
		public PageStyle activePageStyle;

		/// Current Page story telling state
		[HideInInspector]
		public Mode mode = Mode.Idle;

		/// Screen space rect for Page in pixels.
		[HideInInspector]
		public Rect pageRect; 

		/**
		 * Writing speed for page text.
		 */
		public int charactersPerSecond = 60;

		/**
		 * Icon to display when waiting for player input to continue
		 */
		public Texture2D continueIcon;

		/**
		 * Position of continue and swipe icons in normalized screen space coords.
		 * (0,0) = top left, (1,1) = bottom right
		 */
		public Vector2 iconPosition = new Vector2(1,1);

		/**
		 * Default screen position for Page when player enters a Room.
		 */
		public PageController.PagePosition defaultPagePosition;
		
		/**
		 * Default width and height of Page as a fraction of screen height [0..1]
		 */
		public Vector2 defaultPageScale = new Vector2(0.75f, 0.25f);
		
		/**
		 * Automatically center the Page when player is choosing from multiple options.
		 */
		public bool centerChooseMenu = true;
		
		/**
		 * Width of Page as a fraction of screen width [0..1] when automatically centering a Choose menu. 
		 * This setting only has an effect when centerChooseMenu is enabled.
		 */
		public float chooseMenuWidth = 0.5f;

		/**
		 * Sound effect to play when buttons are clicked.
		 */
		public AudioClip clickSound;

		string headerText = "";
		string footerText = "";

		string displayedStoryText = "";
		string originalStoryText = "";

		Action deferredAction;
		Action continueAction;

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

		float quickContinueTimer;

		/**
		 * Translates the PageController specific Mode to the more generic DialogMode.
		 */
		public DialogMode GetDialogMode()
		{
			switch(mode)
			{
			case Mode.Say:
			case Mode.Choose:
				if (FinishedWriting())
				{
					return DialogMode.Waiting;
				}
				else
				{
					return DialogMode.Writing;
				}
			case Mode.Idle:
			default:
				return DialogMode.Idle;
			}
		}

		/**
		 * Calculate a screen space rectangle given normalized screen space coords.
		 * The resulting rect is clamped to always be on-screen.
		 */
		public static Rect CalcPageRect(ScreenRect screenRect)
		{
			Rect rect = new Rect();
			
			rect.xMin = Screen.width * screenRect.x1;
			rect.yMin = Screen.height * screenRect.y1;
			rect.xMax = Screen.width * screenRect.x2;
			rect.yMax = Screen.height * screenRect.y2;
			
			// Clamp to be on-screen
			rect.xMax = Mathf.Min(rect.xMax, Screen.width);
			rect.xMin = Mathf.Max(rect.xMin, 0);
			rect.yMax = Mathf.Min(rect.yMax, Screen.height);
			rect.yMin = Mathf.Max(rect.yMin, 0);
			
			return rect;
		}

		/**
		 * Calculates a screen rect in normalized screen space coordinates in one of the 'standard' Page positions (top, middle, bottom).
		 */
		public static ScreenRect CalcScreenRect(Vector2 pageScale, PagePosition pagePosition)
		{
			float width = Mathf.Clamp01(pageScale.x);
			float height = Mathf.Clamp01(pageScale.y);

			ScreenRect screenRect = new ScreenRect();

			switch (pagePosition)
			{
			case PagePosition.Top:
				screenRect.x1 = 0.5f - width * 0.5f;
				screenRect.x2 = 0.5f + width * 0.5f;
				screenRect.y1 = 0f;
				screenRect.y2 = height;
				break;
			case PagePosition.Middle:
				screenRect.x1 = 0.5f - width * 0.5f;
				screenRect.x2 = 0.5f + width * 0.5f;
				screenRect.y1 = 0.5f - height * 0.5f;
				screenRect.y2 = 0.5f + height * 0.5f;
				break;
			case PagePosition.Bottom:
				screenRect.x1 = 0.5f - width * 0.5f;
				screenRect.x2 = 0.5f + width * 0.5f;
				screenRect.y1 = 1f - Mathf.Clamp01(height);
				screenRect.y2 = 1;
				break;
			}

			return screenRect;
		}

		/**
		 * Reset to the default page layout based on properties in Game class.
		 */
		public void SetDefaultPageLayout()
		{
			ScreenRect screenRect = CalcScreenRect(defaultPageScale, defaultPagePosition);
			pageRect = CalcPageRect(screenRect);
			switch (defaultPagePosition)
			{
			case PageController.PagePosition.Top:
				layout = PageController.Layout.FullSize;
				break;
			case PageController.PagePosition.Middle:
				layout = PageController.Layout.FitToMiddle;
				break;
			case PageController.PagePosition.Bottom:
				layout = PageController.Layout.FullSize;
				break;
			}
		}

		void Update()
		{
			if (quickContinueTimer > 0)
			{
				quickContinueTimer -= Time.deltaTime;
				quickContinueTimer = Mathf.Max(quickContinueTimer, 0f);
			}
		}

		public void SetHeader(string _headerText)
		{
			headerText = _headerText;
		}

		public void SetFooter(string _footerText)
		{
			footerText = _footerText;
		}

		public void Say(string sayText, Action sayAction = null)
		{
			// IDialog does not support the legacy Choose() command
			// Instead, the assumption is that if you call Say() after some options have been added then show the choice menu.
			if (options.Count > 0)
			{
				Choose(sayText);
				return;
			}

			mode = Mode.Say;
			continueAction = sayAction;
			WriteStory(sayText);
		}

		public void ClearOptions()
		{
			options.Clear();
		}

		public void AddOption(string optionText, Action optionAction)
		{
			options.Add(new Option(optionText, optionAction));
		}

		public void Choose(string _chooseText)
		{
			mode = Mode.Choose;
			WriteStory(_chooseText);			
		}

		public void SetTimeout(float _timeoutDuration, Action _timeoutAction)
		{
			Debug.Log("SetTimeout() is not supported by PageController.");
		}

		void WriteStory(string storyText)
		{
			if (activePageStyle == null)
			{
				return;
			}

			// Disable quick continue for a short period to prevent accidental taps
			quickContinueTimer = 0.8f;

			originalStoryText = storyText;

			// Hack to avoid displaying partial color tag text
			if (storyText.Contains("<"))
			{
				displayedStoryText = storyText;
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
			if (charactersPerSecond == 0)
			{
				displayedStoryText = originalStoryText;
				yield break;
			}

			displayedStoryText = "";

			// Make one character visible at a time
			float writeDelay = (1f / (float)charactersPerSecond);
			float timeAccumulator = 0f;
			int i = 0;

			while (true)
			{
				timeAccumulator += Time.deltaTime;

				while (timeAccumulator > writeDelay)
				{
					i++;
					timeAccumulator -= writeDelay;
				}

				if (i >= originalStoryText.Length)
				{
					displayedStoryText = originalStoryText;
					break;
				}
				else
				{
					string left = originalStoryText.Substring(0, i + 1);
					string right = originalStoryText.Substring(i + 1);

					displayedStoryText = left;
					displayedStoryText += "<color=#FFFFFF00>";
					displayedStoryText += right;
					displayedStoryText += "</color>";
				}

				yield return null;
			}
		}

		public bool FinishedWriting()
		{
			return (displayedStoryText.Length == originalStoryText.Length);
		}

		public virtual void OnGUI()
		{
			if (mode == Mode.Idle)
			{
				return;
			}

			if (activePageStyle == null)
			{
				return;
			}

			if (mode == PageController.Mode.Say &&
			    FinishedWriting())
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

			GUIStyle boxStyle = activePageStyle.boxStyle;
			GUIStyle headerStyle = activePageStyle.GetScaledHeaderStyle();
			GUIStyle footerStyle = activePageStyle.GetScaledFooterStyle();
			GUIStyle sayStyle = activePageStyle.GetScaledSayStyle();
			GUIStyle optionStyle = activePageStyle.GetScaledOptionStyle();
			GUIStyle optionAlternateStyle = activePageStyle.GetScaledOptionAlternateStyle();

			Rect outerRect;
			Layout tempLayout;

			if (mode == Mode.Choose &&
				centerChooseMenu)
			{
				// Position the Choose menu in middle of screen
				// The width is controlled by game.chooseMenuWidth
				// The height is automatically fitted to the text content
				Vector2 pageScale = new Vector2(chooseMenuWidth, 0.5f);
				PageController.ScreenRect screenRect = PageController.CalcScreenRect(pageScale, PageController.PagePosition.Middle);
				outerRect = PageController.CalcPageRect(screenRect);
				tempLayout = PageController.Layout.FitToMiddle;
			}
			else
			{
				outerRect = pageRect;
				tempLayout = layout;
			}

			Rect originalRect = outerRect;
			Rect innerRect = CalcInnerRect(outerRect);

			// Calculate height of each section
			float headerHeight = CalcHeaderHeight(innerRect.width);
			float footerHeight = CalcFooterHeight(innerRect.width);
			float storyHeight = CalcStoryHeight(innerRect.width);
			float optionsHeight = CalcOptionsHeight(innerRect.width);
			float contentHeight = headerHeight + footerHeight + storyHeight + optionsHeight;

			// Adjust outer rect position based on alignment settings
			switch (tempLayout)
			{
			case Layout.FullSize:
				outerRect.height = Mathf.Max(outerRect.height, contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom));
				outerRect.y = Mathf.Min(outerRect.y, Screen.height - outerRect.height);
				break;
			case Layout.FitToTop:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = originalRect.yMin;
				break;
			case Layout.FitToMiddle:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = originalRect.center.y - outerRect.height / 2;
				break;
			case Layout.FitToBottom:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = originalRect.yMax - outerRect.height;
				break;
			}

			innerRect = CalcInnerRect(outerRect);

			// Draw box
			Rect boxRect = outerRect;
			boxRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
			if (tempLayout == Layout.FullSize)
			{
				boxRect.height = Mathf.Max(boxRect.height, originalRect.height);
			}
			GUI.Box(boxRect, "", boxStyle);

			// Draw header label
			Rect headerRect = innerRect;
			headerRect.height = headerHeight;
			if (headerHeight > 0)
			{
				GUI.Label(headerRect, headerText, headerStyle);
			}

			// Draw say label
			Rect storyRect = innerRect;
			storyRect.y += headerHeight;
			storyRect.height = storyHeight;
			GUI.Label(storyRect, displayedStoryText, sayStyle);

			// Draw footer label
			Rect footerRect = innerRect;
			footerRect.y += storyHeight;
			footerRect.height = footerHeight;
			if (footerHeight > 0)
			{
				GUI.Label(footerRect, footerText, footerStyle);
			}

			if (!FinishedWriting())
			{
				return;
			}

			// Input handling

			if (mode == Mode.Say)
			{
				// Player can continue by clicking anywhere
				if (quickContinueTimer == 0 &&
				    (Input.GetMouseButtonUp(0) || Input.anyKeyDown) &&
				    continueAction != null)
				{
					deferredAction = continueAction;
				}
			}
			else if (mode == Mode.Choose)
			{
				// Draw option buttons
				Rect buttonRect = innerRect;
				buttonRect.y += headerHeight + storyHeight;
				bool alternateRow = false;
				foreach (Option option in options)
				{
					GUIContent buttonContent = new GUIContent(option.optionText);
					buttonRect.height = optionStyle.CalcHeight(buttonContent, innerRect.width);

					// Select style for odd/even colored rows
					GUIStyle style;
					if (alternateRow)
					{
						style = optionAlternateStyle;
					}
					else
					{
						style = optionStyle;
					}
					alternateRow = !alternateRow;

					if (GUI.Button(buttonRect, buttonContent, style))
					{
						if (option.optionAction != null)
						{
							// We can't execute the option action yet because OnGUI
							// may be called multiple times during a frame, and it's
							// not permitted to modify GUI elements within a frame.
							// We defer executing the action until OnGUI has completed.
							deferredAction = option.optionAction;
							break;
						}
					}

					buttonRect.y += buttonRect.height;
				}
			}

			if (Event.current.type == EventType.Repaint)
			{
				if (deferredAction != null)
				{
					PlayButtonClick();

					Action tempAction = deferredAction;

					displayedStoryText = "";
					originalStoryText = "";
					deferredAction = null;

					if (mode == Mode.Choose)
					{
						ClearOptions();

						// Reset to idle, but calling action may set this again
						mode = Mode.Idle;

						CommandQueue commandQueue = Game.GetInstance().commandQueue;		
						commandQueue.CallCommandMethod(tempAction);
					}
					else if (mode == Mode.Say )
					{
						// Reset to idle, but calling action may set this again
						mode = Mode.Idle;

						// Execute next command
						tempAction();
					}
				}
			}
		}

		float CalcHeaderHeight(float boxWidth)
		{
			if (activePageStyle == null ||
			    mode == Mode.Idle ||
			    headerText.Length == 0)
			{
				return 0;
			}

			GUIStyle headerStyle = activePageStyle.GetScaledHeaderStyle();

			GUIContent headerContent = new GUIContent(headerText);
			return headerStyle.CalcHeight(headerContent, boxWidth);
		}

		float CalcFooterHeight(float boxWidth)
		{
			if (activePageStyle == null ||
			    mode == Mode.Idle ||
			    footerText.Length == 0)
			{
				return 0;
			}
			
			GUIStyle footerStyle = activePageStyle.GetScaledFooterStyle();
			
			GUIContent headerContent = new GUIContent(headerText);
			return footerStyle.CalcHeight(headerContent, boxWidth);
		}

		float CalcStoryHeight(float boxWidth)
		{
			GUIStyle sayStyle = activePageStyle.GetScaledSayStyle();

			if (activePageStyle == null ||
			    mode == Mode.Idle || 
			    originalStoryText.Length == 0)
			{
				// Allow a space for story even if there's no text
				return sayStyle.lineHeight;
			}

			GUIContent storyContent = new GUIContent(originalStoryText + "\n");
			return sayStyle.CalcHeight(storyContent, boxWidth);
		}

		float CalcOptionsHeight(float boxWidth)
		{
			if (activePageStyle == null ||
			    mode == Mode.Idle ||
			    options.Count == 0)
			{
				return 0;
			}

			// This assumes that the alternate option style is the same height as the regular style
			GUIStyle optionStyle = activePageStyle.GetScaledOptionStyle();

			float totalHeight = 0;
			foreach (Option option in options)
			{
				GUIContent optionContent = new GUIContent(option.optionText);
				float optionHeight = optionStyle.CalcHeight(optionContent, boxWidth);
				totalHeight += optionHeight;
			}

			// Add space at bottom
			GUIStyle sayStyle = activePageStyle.GetScaledSayStyle();
			totalHeight += sayStyle.lineHeight;

			return totalHeight;
		}

		// Returns smaller internal box rect with padding style applied
		Rect CalcInnerRect(Rect outerRect)
		{
			if (activePageStyle == null)
			{
				return new Rect();
			}

			GUIStyle boxStyle = activePageStyle.boxStyle;

			Rect innerRect = new Rect(outerRect.x + boxStyle.padding.left,
			                		  outerRect.y + boxStyle.padding.top,
			                          outerRect.width - (boxStyle.padding.left + boxStyle.padding.right),
			                          outerRect.height - (boxStyle.padding.top + boxStyle.padding.bottom));

			return innerRect;
		}

		/**
		 * Plays the button clicked sound effect
		 */
		public void PlayButtonClick()
		{
			if (clickSound != null)
			{
				audio.PlayOneShot(clickSound);
			}
		}
	}
}