using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	// A rectangular screen area for rendering story text.
	// Rooms may contain any number of Pages.
	// If a Page is a child of a View, then transitiong to that View will automatically activate the Page.
	[ExecuteInEditMode]
	public class Page : MonoBehaviour 
	{
		public Bounds pageBounds = new Bounds(Vector3.zero, new Vector3(0.25f, 0.25f, 0f));

		// The font size for title, say and option text is calculated by multiplying the screen height
		// by the corresponding font scale. Text appears the same size across all device resolutions.
		public float titleFontScale = 1f / 20f;
		public float sayFontScale = 1f / 25f;
		public float optionFontScale = 1f / 25f;

		public GUIStyle titleStyle;
		public GUIStyle sayStyle;
		public GUIStyle optionStyle;
		public GUIStyle boxStyle;

		string titleText = "";

		string originalStoryText = "";
		string displayedStoryText = "";

		Action deferredAction;

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

		void Start()
		{
			// Override the font size to compensate for varying device resolution
			// Font size is calculated as a fraction of the current screen height
			titleStyle.fontSize = Mathf.RoundToInt((float)Screen.height * titleFontScale);
			sayStyle.fontSize = Mathf.RoundToInt((float)Screen.height * sayFontScale);
			optionStyle.fontSize = Mathf.RoundToInt((float)Screen.height * optionFontScale);
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
			WriteStory(subbedText, sayAction);
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
			WriteStory(subbedText, null);			
		}

		void WriteStory(string storyText, Action writeAction)
		{
			// Add continue option
			if (writeAction != null)
			{
				options.Clear();
				options.Add(new Option(Game.GetInstance().continueText, writeAction));
			}

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

			Rect pageRect = GetScreenRect();
			Rect outerRect = pageRect;
			Rect innerRect = CalcInnerRect(outerRect);

			// Calculate height of each section
			float titleHeight = CalcTitleHeight(innerRect.width);
			float storyHeight = CalcStoryHeight(innerRect.width);
			float optionsHeight = CalcOptionsHeight(innerRect.width);
			float contentHeight = titleHeight + storyHeight + optionsHeight;

			// Adjust inner and outer rect to center around original page middle
			outerRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
			outerRect.y = pageRect.center.y - outerRect.height / 2;
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

			// Draw option buttons
			bool finishedWriting = (displayedStoryText.Length == originalStoryText.Length);

			if (finishedWriting)
			{
				// Player can continue through single options by clicking / tapping anywhere
				bool quickContinue = (options.Count == 1 && (Input.GetMouseButtonUp(0) || Input.anyKeyDown));

				Rect buttonRect = innerRect;
				buttonRect.y += titleHeight + storyHeight;
				foreach (Option option in options)
				{
					GUIContent buttonContent = new GUIContent(option.optionText);
					buttonRect.height = optionStyle.CalcHeight(buttonContent, innerRect.width);

					if (quickContinue || 
					    GUI.Button(buttonRect, buttonContent, optionStyle))
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
					else if (mode == Mode.Say)
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
			if (mode == Mode.Idle ||
			    titleText.Length == 0)
			{
				return 0;
			}

			GUIContent titleContent = new GUIContent(titleText);
			return titleStyle.CalcHeight(titleContent, boxWidth);
		}

		float CalcStoryHeight(float boxWidth)
		{
			if (mode == Mode.Idle || 
			    originalStoryText.Length == 0)
			{
				return 0;
			}

			GUIContent storyContent = new GUIContent(originalStoryText + "\n");
			return sayStyle.CalcHeight(storyContent, boxWidth);
		}

		float CalcOptionsHeight(float boxWidth)
		{
			if (mode == Mode.Idle ||
			    options.Count == 0)
			{
				return 0;
			}
			
			float totalHeight = 0;
			foreach (Option option in options)
			{
				GUIContent optionContent = new GUIContent(option.optionText);
				float optionHeight = optionStyle.CalcHeight(optionContent, boxWidth);
				totalHeight += optionHeight;
			}

			return totalHeight;
		}

		// Returns smaller internal box rect with padding style applied
		Rect CalcInnerRect(Rect outerRect)
		{
			return new Rect(outerRect.x + boxStyle.padding.left,
			                outerRect.y + boxStyle.padding.top,
			                outerRect.width - (boxStyle.padding.left + boxStyle.padding.right),
			                outerRect.height - (boxStyle.padding.top + boxStyle.padding.bottom));
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