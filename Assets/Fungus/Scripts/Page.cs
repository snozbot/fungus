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

			Rect pageRect = GetScreenRect();
			Rect outerRect = FitRectToScreen(pageRect);
			Rect innerRect = CalcInnerRect(outerRect);

			// Calculate height of each section
			float headerHeight = CalcHeaderHeight(innerRect.width);
			float footerHeight = CalcFooterHeight(innerRect.width);
			float storyHeight = CalcStoryHeight(innerRect.width);
			float optionsHeight = CalcOptionsHeight(innerRect.width);
			float contentHeight = headerHeight + footerHeight + storyHeight + optionsHeight;

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
			outerRect = FitRectToScreen(outerRect);

			innerRect = CalcInnerRect(outerRect);

			// Draw box
			Rect boxRect = outerRect;
			boxRect.height = contentHeight + (boxStyle.padding.top + boxStyle.padding.bottom);
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

			bool finishedWriting = (displayedStoryText.Length == originalStoryText.Length);
			if (!finishedWriting)
			{
				return;
			}

			if (mode == Mode.Say)
			{
				ContinueStyle continueStyle = Game.GetInstance().continueStyle;
				if (continueStyle != null)
				{
					DrawContinueButton(outerRect);
				}

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

		// Force rect to always be on-screen
		Rect FitRectToScreen(Rect rect)
		{
			Rect fittedRect = new Rect();

			fittedRect.xMax = Mathf.Min(rect.xMax, Screen.width);
			fittedRect.xMin = Mathf.Max(rect.xMin, 0);
			fittedRect.yMax = Mathf.Min(rect.yMax, Screen.height);
			fittedRect.yMin = Mathf.Max(rect.yMin, 0);

			return fittedRect;
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

		/**
		 * Returns the page rect in screen space coords
		 */
		public Rect GetScreenRect()
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
			
			Rect pageRect = new Rect(tl.x, Screen.height - tl.y, br.x - tl.x, tl.y - br.y);

			return FitRectToScreen(pageRect);
		}

		void DrawContinueButton(Rect containerRect)
		{
			PageStyle pageStyle = Game.GetInstance().activePageStyle;
			ContinueStyle continueStyle = Game.GetInstance().continueStyle;

			if (pageStyle == null ||
				continueStyle == null)
			{
				return;
			}

			GUIStyle style = continueStyle.style;
			if (style == null)
			{
				return;
			}

			GUIContent content = new GUIContent(continueStyle.continueText);
			GUIStyle scaledContinueStyle = continueStyle.GetScaledContinueStyle();

			Rect continueRect;

			if (continueStyle.onPage)
			{
				float width = scaledContinueStyle.CalcSize(content).x;
				float height = scaledContinueStyle.lineHeight;
				float x = containerRect.xMin + (containerRect.width) - (width) - pageStyle.boxStyle.padding.right;
				float y = containerRect.yMax - height / 2f;
				continueRect = new Rect(x, y, width, height);
			}
			else
			{
				Vector2 size = scaledContinueStyle.CalcSize(content);
				
				float x = Screen.width * continueStyle.screenPosition.x;
				float y = Screen.height * continueStyle.screenPosition.y;
				float width = size.x;
				float height = size.y;
				
				x = Mathf.Max(x, continueStyle.padding.x);
				x = Mathf.Min(x, Screen.width - width - continueStyle.padding.x); 
				
				y = Mathf.Max(y, continueStyle.padding.y);
				y = Mathf.Min(y, Screen.height - height - continueStyle.padding.y); 
				
				continueRect = new Rect(x, y, width, height);
			}

			GUI.Label(continueRect, content, scaledContinueStyle);
		}
	}
}