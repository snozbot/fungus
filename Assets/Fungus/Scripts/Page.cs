using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	/**
	 * A rectangular screen area for rendering story text.
	 * Rooms may contain any number of Pages.
	 * If a Page is a child of a View, then transitioning to that View will automatically activate the Page.
	 */
	[ExecuteInEditMode]
	public class Page : MonoBehaviour 
	{
		/// Page alignment options
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

		/// Page position within bounds when display height is less than bounds height.
		public Layout layout = Layout.FullSize;

		string headerText = "";
		string footerText = "";

		string displayedStoryText = "";
		string originalStoryText = "";

		Action deferredAction;
		Action continueAction;

		public enum Mode
		{
			Idle,
			Say,
			Choose
		};

		[HideInInspector]
		public Mode mode = Mode.Idle;

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

		Rect pageRect; // Screen space rect for Page in pixels

		/**
		 * Set the screen rect in normalized screen space coords.
		 * The origin is at the top left of the screen.
		 */
		public void SetPageRect(float x1, float y1, float x2, float y2)
		{
			pageRect.xMin = Screen.width * x1;
			pageRect.yMin = Screen.height * y1;
			pageRect.xMax = Screen.width * x2;
			pageRect.yMax = Screen.height * y2;

			// Clamp to be on-screen
			pageRect.xMax = Mathf.Min(pageRect.xMax, Screen.width);
			pageRect.xMin = Mathf.Max(pageRect.xMin, 0);
			pageRect.yMax = Mathf.Min(pageRect.yMax, Screen.height);
			pageRect.yMin = Mathf.Max(pageRect.yMin, 0);
		}

		public virtual void Update()
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

		public void Say(string sayText, Action sayAction)
		{
			mode = Mode.Say;
			StringTable stringTable = Game.GetInstance().stringTable;
			string subbedText = stringTable.SubstituteStrings(sayText);
			continueAction = sayAction;
			WriteStory(subbedText);
		}

		public void AddOption(string optionText, Action optionAction)
		{
			StringTable stringTable = Game.GetInstance().stringTable;
			string subbedText = stringTable.FormatLinkText(stringTable.SubstituteStrings(optionText));
			options.Add(new Option(subbedText, optionAction));
		}

		public void Choose(string _chooseText)
		{
			mode = Mode.Choose;
			StringTable stringTable = Game.GetInstance().stringTable;
			string subbedText = stringTable.SubstituteStrings(_chooseText);
			WriteStory(subbedText);			
		}

		void WriteStory(string storyText)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			if (pageStyle == null)
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
			int charactersPerSecond = Game.GetInstance().charactersPerSecond;

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

			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			if (pageStyle == null)
			{
				return;
			}

			GUIStyle boxStyle = pageStyle.boxStyle;
			GUIStyle headerStyle = pageStyle.GetScaledHeaderStyle();
			GUIStyle footerStyle = pageStyle.GetScaledFooterStyle();
			GUIStyle sayStyle = pageStyle.GetScaledSayStyle();
			GUIStyle optionStyle = pageStyle.GetScaledOptionStyle();
			GUIStyle optionAlternateStyle = pageStyle.GetScaledOptionAlternateStyle();

			Rect outerRect = pageRect;
			Rect innerRect = CalcInnerRect(outerRect);

			// Calculate height of each section
			float headerHeight = CalcHeaderHeight(innerRect.width);
			float footerHeight = CalcFooterHeight(innerRect.width);
			float storyHeight = CalcStoryHeight(innerRect.width);
			float optionsHeight = CalcOptionsHeight(innerRect.width);
			float contentHeight = headerHeight + footerHeight + storyHeight + optionsHeight;

			// Adjust outer rect position based on alignment settings
			switch (layout)
			{
			case Layout.FullSize:
				outerRect.height = Mathf.Max(outerRect.height, contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom));
				outerRect.y = Mathf.Min(outerRect.y, Screen.height - outerRect.height);
				break;
			case Layout.FitToTop:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.yMin;
				break;
			case Layout.FitToMiddle:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.center.y - outerRect.height / 2;
				break;
			case Layout.FitToBottom:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.yMax - outerRect.height;
				break;
			}

			innerRect = CalcInnerRect(outerRect);

			// Draw box
			Rect boxRect = outerRect;
			boxRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
			if (layout == Layout.FullSize)
			{
				boxRect.height = Mathf.Max(boxRect.height, pageRect.height);
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
					Game.GetInstance().PlayButtonClick();

					Action tempAction = deferredAction;

					displayedStoryText = "";
					originalStoryText = "";
					deferredAction = null;

					if (mode == Mode.Choose)
					{
						options.Clear();

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
			PageStyle pageStyle = Game.GetInstance().activePageStyle;

			if (pageStyle == null ||
			    mode == Mode.Idle ||
			    headerText.Length == 0)
			{
				return 0;
			}

			GUIStyle headerStyle = pageStyle.GetScaledHeaderStyle();

			GUIContent headerContent = new GUIContent(headerText);
			return headerStyle.CalcHeight(headerContent, boxWidth);
		}

		float CalcFooterHeight(float boxWidth)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			
			if (pageStyle == null ||
			    mode == Mode.Idle ||
			    footerText.Length == 0)
			{
				return 0;
			}
			
			GUIStyle footerStyle = pageStyle.GetScaledFooterStyle();
			
			GUIContent headerContent = new GUIContent(headerText);
			return footerStyle.CalcHeight(headerContent, boxWidth);
		}

		float CalcStoryHeight(float boxWidth)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			GUIStyle sayStyle = pageStyle.GetScaledSayStyle();

			if (pageStyle == null ||
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
			PageStyle pageStyle = Game.GetInstance().activePageStyle;

			if (pageStyle == null ||
			    mode == Mode.Idle ||
			    options.Count == 0)
			{
				return 0;
			}

			// This assumes that the alternate option style is the same height as the regular style
			GUIStyle optionStyle = pageStyle.GetScaledOptionStyle();

			float totalHeight = 0;
			foreach (Option option in options)
			{
				GUIContent optionContent = new GUIContent(option.optionText);
				float optionHeight = optionStyle.CalcHeight(optionContent, boxWidth);
				totalHeight += optionHeight;
			}

			// Add space at bottom
			GUIStyle sayStyle = pageStyle.GetScaledSayStyle();
			totalHeight += sayStyle.lineHeight;

			return totalHeight;
		}

		// Returns smaller internal box rect with padding style applied
		Rect CalcInnerRect(Rect outerRect)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;

			if (pageStyle == null)
			{
				return new Rect();
			}

			GUIStyle boxStyle = pageStyle.boxStyle;

			Rect innerRect = new Rect(outerRect.x + boxStyle.padding.left,
			                		  outerRect.y + boxStyle.padding.top,
			                          outerRect.width - (boxStyle.padding.left + boxStyle.padding.right),
			                          outerRect.height - (boxStyle.padding.top + boxStyle.padding.bottom));

			return innerRect;
		}
	}
}