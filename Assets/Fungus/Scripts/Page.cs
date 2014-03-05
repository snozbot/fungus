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
		/// Rectangular bounds used to display page text
		public Bounds pageBounds = new Bounds(Vector3.zero, new Vector3(0.25f, 0.25f, 0f));

		/// Page position within bounds when display height is less than bounds height
		public enum VerticalAlign
		{
			Top,
			Middle,
			Bottom
		}

		public VerticalAlign verticalAlign = VerticalAlign.Middle;

		string titleText = "";

		string originalStoryText = "";
		string displayedStoryText = "";

		Action deferredAction;
		Action continueAction;

		public enum Mode
		{
			Idle,
			Say,
			Choose
		};

		Mode mode = Mode.Idle;

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

		public virtual void Update()
		{
			if (quickContinueTimer > 0)
			{
				quickContinueTimer -= Time.deltaTime;
				quickContinueTimer = Mathf.Max(quickContinueTimer, 0f);
			}
		}

		public void SetTitle(string _titleText)
		{
			titleText = _titleText;
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

			GUIStyle sayStyle = pageStyle.GetScaledSayStyle();

			// Disable quick continue for a short period to prevent accidental taps
			quickContinueTimer = 0.8f;

			// Hack to avoid displaying partial color tag text
			if (storyText.Contains("<"))
			{
				originalStoryText = storyText;
				displayedStoryText = storyText;
			}
			else
			{
				float textWidth = CalcInnerRect(GetScreenRect()).width;
				originalStoryText = InsertLineBreaks(storyText, sayStyle, textWidth);
				displayedStoryText = "";

				// Use a coroutine to write the story text out over time
				StartCoroutine(WriteStoryInternal());
			}
		}

		// Coroutine to write story text out over a period of time
		IEnumerator WriteStoryInternal()
		{
			int charactersPerSecond = Game.GetInstance().charactersPerSecond;

			// Zero CPS means write instantly
			if (charactersPerSecond <= 0)
			{
				displayedStoryText = originalStoryText;
				yield break;
			}

			displayedStoryText = "";
			float writeDelay = 1f / (float)charactersPerSecond;
			float timeAccumulator = 0f;

			while (displayedStoryText.Length < originalStoryText.Length)
			{
				timeAccumulator += Time.deltaTime;

				while (timeAccumulator > 0f)
				{
					timeAccumulator -= writeDelay;

					if (displayedStoryText.Length < originalStoryText.Length)
					{
						displayedStoryText += originalStoryText.Substring(displayedStoryText.Length, 1);
					}
				}

				yield return null;
			}

			displayedStoryText = originalStoryText;
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
			GUIStyle titleStyle = pageStyle.GetScaledTitleStyle();
			GUIStyle sayStyle = pageStyle.GetScaledSayStyle();
			GUIStyle optionStyle = pageStyle.GetScaledOptionStyle();
			GUIStyle optionAlternateStyle = pageStyle.GetScaledOptionAlternateStyle();
			GUIStyle continueStyle = pageStyle.GetScaledContinueStyle();

			Rect pageRect = GetScreenRect();
			Rect outerRect = pageRect;
			Rect innerRect = CalcInnerRect(outerRect);

			// Calculate height of each section
			float titleHeight = CalcTitleHeight(innerRect.width);
			float storyHeight = CalcStoryHeight(innerRect.width);
			float optionsHeight = CalcOptionsHeight(innerRect.width);
			float contentHeight = titleHeight + storyHeight + optionsHeight;

			// Adjust outer rect position based on alignment settings
			switch (verticalAlign)
			{
			case VerticalAlign.Top:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.yMin;
				break;
			case VerticalAlign.Middle:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.center.y - outerRect.height / 2;
				break;
			case VerticalAlign.Bottom:
				outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
				outerRect.y = pageRect.yMax - outerRect.height;
				break;
			}

			// Force outer rect to always be on-screen
			// If the rect is bigger than the screen, then the top-left corner will always be visible
			outerRect.x = Mathf.Min(outerRect.x, Screen.width - outerRect.width);
			outerRect.y = Mathf.Min(outerRect.y, Screen.height - outerRect.height);
			outerRect.x = Mathf.Max(0, outerRect.x);
			outerRect.y = Mathf.Max(0, outerRect.y);

			innerRect = CalcInnerRect(outerRect);

			// Draw box
			Rect boxRect = outerRect;
			boxRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
			GUI.Box(boxRect, "", boxStyle);

			// Draw title label
			Rect titleRect = innerRect;
			titleRect.height = titleHeight;
			GUI.Label(titleRect, titleText, titleStyle);

			// Draw say label
			Rect storyRect = innerRect;
			storyRect.y += titleHeight;
			storyRect.height = storyHeight;
			GUI.Label(storyRect, displayedStoryText, sayStyle);

			bool finishedWriting = (displayedStoryText.Length == originalStoryText.Length);
			if (!finishedWriting)
			{
				return;
			}

			if (mode == Mode.Say)
			{
				Rect continueRect = CalcContinueRect(outerRect);
				GUI.Button(continueRect, new GUIContent(Game.GetInstance().continueText), continueStyle);

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
				buttonRect.y += titleHeight + storyHeight;
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
					Action tempAction = deferredAction;

					options.Clear();
					displayedStoryText = "";
					originalStoryText = "";
					deferredAction = null;

					if (mode == Mode.Choose)
					{
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

		float CalcTitleHeight(float boxWidth)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;

			if (pageStyle == null ||
			    mode == Mode.Idle ||
			    titleText.Length == 0)
			{
				return 0;
			}

			GUIStyle titleStyle = pageStyle.GetScaledTitleStyle();

			GUIContent titleContent = new GUIContent(titleText);
			return titleStyle.CalcHeight(titleContent, boxWidth);
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

			return new Rect(outerRect.x + boxStyle.padding.left,
			                outerRect.y + boxStyle.padding.top,
			                outerRect.width - (boxStyle.padding.left + boxStyle.padding.right),
			                outerRect.height - (boxStyle.padding.top + boxStyle.padding.bottom));
		}

		Rect CalcContinueRect(Rect outerRect)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			
			if (pageStyle == null)
			{
				return new Rect();
			}

			GUIStyle continueStyle = pageStyle.GetScaledContinueStyle();

			GUIContent content = new GUIContent(Game.GetInstance().continueText);
			float width = continueStyle.CalcSize(content).x;
			float height = continueStyle.lineHeight;

			float x = outerRect.xMin + (outerRect.width) - (width) - pageStyle.boxStyle.padding.right;
			float y = outerRect.yMax - height / 2f;

			return new Rect(x, y, width, height);
		}

		// Returns the page rect in screen space coords
		Rect GetScreenRect()
		{
			// Y decreases up the screen in GUI space, so top left is rect origin
			
			Vector3 topLeft = transform.position + pageBounds.center;
			topLeft.x -= pageBounds.extents.x;
			topLeft.y += pageBounds.extents.y;
			
			Vector3 bottomRight = transform.position + pageBounds.center;
			bottomRight.x += pageBounds.extents.x;
			bottomRight.y -= pageBounds.extents.y;
			
			Camera mainCamera = GameObject.FindGameObjectWithTag("MainCamera").camera;
			Vector2 tl = mainCamera.WorldToScreenPoint(topLeft);
			Vector2 br = mainCamera.WorldToScreenPoint(bottomRight);
			
			return new Rect(tl.x, Screen.height - tl.y, br.x - tl.x, tl.y - br.y);
		}

		// Inserts extra line breaks to avoid partial words 'jumping' to next line due to word wrap
		string InsertLineBreaks(string text, GUIStyle style, float maxWidth)
		{
			string output = "";
			string[] parts = Regex.Split(text, @"(?=\s)");
			foreach (string word in parts)
			{
				float oldHeight = style.CalcHeight(new GUIContent(output), maxWidth);
				float newHeight = style.CalcHeight(new GUIContent(output + word), maxWidth);
				
				if (oldHeight > 0 &&
				    newHeight > oldHeight)
				{
					output += "\n" + word.TrimStart();
				}
				else
				{
					output += word;
				}				
			}
			
			return output;
		}
	}
}