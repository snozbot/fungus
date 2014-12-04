using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus
{
	
	public class Dialog : MonoBehaviour 
	{
		public float writingSpeed = 60;
		public AudioClip writingSound;
		public bool loopWritingSound = true;
		public float punctuationPause = 0.25f;

		[Tooltip("Click anywhere on screen to continue when set to true, or only on dialog when false.")]
		public bool clickAnywhere = true;
		
		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image characterImage;
		public AudioClip characterTypingSound;

		protected float currentSpeed;
		protected float currentPunctuationPause;
		protected bool boldActive;
		protected bool italicActive;
		protected bool colorActive;
		protected string colorText;
		protected float clickCooldownTimer;

		protected bool wasPointerClicked;

		protected enum TokenType
		{
			Character,				// Text character
			BoldStart,				// b
			BoldEnd,				// /b
			ItalicStart,			// i
			ItalicEnd,				// /i
			ColorStart,				// color=red
			ColorEnd,				// /color
			Wait, 					// w, w=0.5
			WaitForInputNoClear, 	// wi
			WaitForInputAndClear, 	// wc
			WaitOnPunctuation, 		// wp, wp=0.5
			Clear, 					// c
			Speed, 					// s, s=60
			Exit 					// x
		}

		protected class Token
		{
			public TokenType type = TokenType.Character;
			public string param = "";
		}

		protected virtual void LateUpdate()
		{
			wasPointerClicked = false;

			if (clickCooldownTimer > 0f)
			{
				clickCooldownTimer -= Time.deltaTime;
				clickCooldownTimer = Mathf.Max(0, clickCooldownTimer); 
			}

			if (clickCooldownTimer == 0f &&
			    clickAnywhere &&
			    Input.GetMouseButtonDown(0))
			{
				wasPointerClicked = true;
				clickCooldownTimer = 0.2f;
			}
		}

		public virtual void ShowDialog(bool visible)
		{
			if (dialogCanvas != null)
			{
				dialogCanvas.gameObject.SetActive(visible);
			}
		}
		
		public virtual void SetCharacter(Character character)
		{
			if (character == null)
			{
				if (characterImage != null)
					characterImage.enabled = false;
				if (nameText != null)
					nameText.text = "";
				characterTypingSound = null;
			}
			else
			{
				SetCharacterImage(character.profileSprite);

				string characterName = character.nameText;
				if (characterName == "")
				{
					// Use game object name as default
					characterName = character.name;
				}

				characterTypingSound = character.soundEffect;

				SetCharacterName(characterName, character.nameColor);
			}

		}
		
		public virtual void SetCharacterImage(Sprite image)
		{
			if (characterImage != null)
			{
				if (image != null)
				{
					characterImage.sprite = image;
					characterImage.enabled = true;
				}
				else
				{
					characterImage.enabled = false;
				}
			}
		}
		
		public virtual void SetCharacterName(string name, Color color)
		{
			if (nameText != null)
			{
				nameText.text = name;
				nameText.color = color;
			}
		}

		protected virtual IEnumerator WriteText(string text, Action onWritingComplete, Action onExitTag)
		{
			storyText.text = "";
			boldActive = false;
			italicActive = false;
			colorActive = false;
			colorText = "";
			currentSpeed = writingSpeed;
			currentPunctuationPause = punctuationPause;

			List<Token> tokens = MakeTokenList(text);

			if (tokens.Count == 0)
			{
				if (onWritingComplete != null)
				{
					onWritingComplete();
				}
				yield break;
			}

			// Zero speed means write instantly
			if (currentSpeed == 0 ||
			    text.Contains("<"))
			{
				currentSpeed = 10000;
			}

			GameObject typingAudio = null;
			
			if (writingSound != null)
			{
				typingAudio = new GameObject("WritingSound");
				typingAudio.AddComponent<AudioSource>();
				if (characterTypingSound != null)
				{
					typingAudio.audio.clip = characterTypingSound;
				}
				else
				{
					typingAudio.audio.clip = writingSound;
				}
				typingAudio.audio.loop = loopWritingSound;
				typingAudio.audio.Play();
			}

			float timeAccumulator = 0f;

			int i = 0;
			while (i < tokens.Count)
			{
				timeAccumulator += Time.deltaTime;

				float writeDelay = 0f;
				if (currentSpeed > 0)
				{
					writeDelay = (1f / (float)currentSpeed);
				}

				while (timeAccumulator > writeDelay)
				{
					timeAccumulator -= writeDelay;

					Token token = tokens[i];

					switch (token.type)
					{
					case TokenType.Character:

						if (storyText.text.Length == 0 && token.param == "\n")
						{
							// Ignore leading newlines
						}
						else
						{
							// Wrap each individual character in rich text markup tags if required
							// This must be done at the character level to support writing out the story text over time.
							string start = "";
							string end = "";
							if (boldActive)
							{
								start += "<b>"; 
								end += "</b>";
							}
							if (italicActive)
							{
								start += "<i>"; 
								end = "</i>" + end; // Have to nest tags correctly 
							}
							if (colorActive)
							{
								start += "<color=" + colorText + ">"; 
								end += "</color>"; 
							}
			
							storyText.text += start + token.param + end;

							if (wasPointerClicked)
							{
								currentSpeed = 10000; // Write instantly
								wasPointerClicked = false;
							}
						}

						// Add a wait token on punctuation marks
						bool doPause = punctuationPause > 0 && IsPunctuation(token.param);
						if (i == tokens.Count - 1)
						{
							doPause = false; // No pause on last character
						}
						else 
						{
							// No pause if next token is a pause
							TokenType nextType = tokens[i + 1].type;
							if (nextType == TokenType.Wait ||
							    nextType == TokenType.WaitForInputAndClear ||
							    nextType == TokenType.WaitForInputNoClear)
							{
								doPause = false;
							}

							if (currentSpeed > 1000)
							{
								doPause = false;
							}
						}

						if (doPause)
						{
							// Ignore if next token is also punctuation, or if punctuation is the last character.
							bool skipCharacter = (i < tokens.Count - 1 &&
							             tokens[i + 1].type == TokenType.Character &&
							             IsPunctuation(tokens[i + 1].param));

							if (!skipCharacter)
							{
								if (typingAudio != null)
									typingAudio.audio.Pause();

								yield return new WaitForSeconds(currentPunctuationPause);

								if (typingAudio != null)
									typingAudio.audio.Play();
							}
						}

						break;

					case TokenType.BoldStart:
						boldActive = true;
						break;
						
					case TokenType.BoldEnd:
						boldActive = false;
						break;
					
					case TokenType.ItalicStart:
						italicActive = true;
						break;
						
					case TokenType.ItalicEnd:
						italicActive = false;
						break;

					case TokenType.ColorStart:
						colorActive = true;
						colorText = token.param;
						break;
						
					case TokenType.ColorEnd:
						colorActive = false;
						break;

					case TokenType.Wait:
						float duration = 1f;
						if (!Single.TryParse(token.param, out duration))
						{
							duration = 1f;
						}

						if (typingAudio != null)
							typingAudio.audio.Pause();

						yield return new WaitForSeconds(duration);

						if (typingAudio != null)
							typingAudio.audio.Play();

						timeAccumulator = 0f;
						break;

					case TokenType.WaitForInputNoClear:
						OnWaitForInputTag(true);
						if (typingAudio != null)
							typingAudio.audio.Pause();

						yield return StartCoroutine(WaitForInput(null));

						if (typingAudio != null)
							typingAudio.audio.Play();

						timeAccumulator = 0f;
						currentSpeed = writingSpeed;
						OnWaitForInputTag(false);
						break;
					
					case TokenType.WaitForInputAndClear:
						OnWaitForInputTag(true);

						if (typingAudio != null)
							typingAudio.audio.Pause();

						yield return StartCoroutine(WaitForInput(null));

						if (typingAudio != null)
							typingAudio.audio.Play();

						OnWaitForInputTag(false);
						timeAccumulator = 0f;
						currentSpeed = writingSpeed;
						storyText.text = "";
						break;

					case TokenType.Clear:
						storyText.text = "";
						timeAccumulator = 0f;
						break;

					case TokenType.Speed:
						if (!Single.TryParse(token.param, out currentSpeed))
						{
							currentSpeed = 0f;
						}
						writeDelay = 0;
						timeAccumulator = 0f;
						if (currentSpeed > 0)
						{
							writeDelay = (1f / (float)currentSpeed);
						}
						break;

					case TokenType.Exit:

						if (typingAudio != null)
						{
							Destroy(typingAudio);
						}

						if (onExitTag != null)
						{
							onExitTag();
						}

						yield break;

					case TokenType.WaitOnPunctuation:
						if (!Single.TryParse(token.param, out currentPunctuationPause))
						{
							currentPunctuationPause = 0f;
						}
						break;
					}

					if (++i >= tokens.Count)
					{
						break;
					}			
				}

				yield return null;
			}
			
			if (typingAudio != null)
			{
				Destroy(typingAudio);
			}
			
			if (onWritingComplete != null)
			{
				onWritingComplete();
			}
			
			yield break;
		}

		public virtual void Clear()
		{
			ClearStoryText();

			// Reset control variables
			currentSpeed = 60;
			currentPunctuationPause = 0.25f;
			boldActive = false;
			italicActive = false;
			colorActive = false;
			colorText = "";

			// Kill any active write coroutine
			StopAllCoroutines();
		}
		
		protected virtual void ClearStoryText()
		{
			if (storyText != null)
			{
				storyText.text = "";
			}
		}

		protected virtual bool IsPunctuation(string character)
		{
			return character == "." ||
				character == "?" ||
				character == "!";
		}
		
		protected virtual List<Token> MakeTokenList(string storyText)
		{
			List<Token> tokenList = new List<Token>();

			string pattern = @"\{.*?\}";
			Regex myRegex = new Regex(pattern);

			Match m = myRegex.Match(storyText);   // m is the first match
			
			int position = 0;
			while (m.Success)
			{
				// Get bit leading up to tag
				string preText = storyText.Substring(position, m.Index - position);
				string tagText = m.Value;
			
				foreach (char c in preText)
				{
					AddCharacterToken(tokenList, c.ToString());
				}

				AddTagToken(tokenList, tagText);

				position = m.Index + tagText.Length;
				m = m.NextMatch();
			}
			
			if (position < storyText.Length - 1)
			{
				string postText = storyText.Substring(position, storyText.Length - position);
				foreach (char c in postText)
				{
					AddCharacterToken(tokenList, c.ToString());
				}
			}

			return tokenList;
		}

		protected virtual void AddCharacterToken(List<Token> tokenList, string character)
		{
			Token token = new Token();
			token.type = TokenType.Character;
			token.param = character;
			tokenList.Add(token);
		}

		protected virtual void AddTagToken(List<Token> tokenList, string tagText)
		{
			if (tagText.Length < 3 ||
			    tagText.Substring(0,1) != "{" ||
			    tagText.Substring(tagText.Length - 1,1) != "}")
			{
				return;
			}

			string tag = tagText.Substring(1, tagText.Length - 2);

			TokenType type = TokenType.Character;
			string paramText = "";

			if (tag == "b")
			{
				type = TokenType.BoldStart;
			}
			else if (tag == "/b")
			{
				type = TokenType.BoldEnd;
			}
			else if (tag == "i")
			{
				type = TokenType.ItalicStart;
			}
			else if (tag == "/i")
			{
				type = TokenType.ItalicEnd;
			}
			else if (tag.StartsWith("color="))
			{
				type = TokenType.ColorStart;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag == "/color")
			{
				type = TokenType.ColorEnd;
			}
			else if (tag == "wi")
			{
				type = TokenType.WaitForInputNoClear;
			}
			if (tag == "wc")
			{
				type = TokenType.WaitForInputAndClear;
			}
			else if (tag.StartsWith("wp="))
			{
				type = TokenType.WaitOnPunctuation;
				paramText = tag.Substring(3, tag.Length - 3);
			}
			else if (tag == "wp")
			{
				type = TokenType.WaitOnPunctuation;
			}
			else if (tag.StartsWith("w="))
			{
				type = TokenType.Wait;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "w")
			{
				type = TokenType.Wait;
			}
			else if (tag == "c")
			{
				type = TokenType.Clear;
			}
			else if (tag.StartsWith("s="))
			{
				type = TokenType.Speed;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "s")
			{
				type = TokenType.Speed;
			}
			else if (tag == "x")
			{
				type = TokenType.Exit;
			}

			Token token = new Token();
			token.type = type;
			token.param = paramText.Trim();

			tokenList.Add(token);
		}

		protected virtual IEnumerator WaitForInput(Action onInput)
		{
			while (!wasPointerClicked)
			{
				yield return null;
			}

			wasPointerClicked = false;

			if (onInput != null)
			{
				onInput();
			}
		}

		protected virtual void OnWaitForInputTag(bool waiting)
		{}

		public virtual void OnPointerClick()
		{
			if (clickCooldownTimer == 0f)
			{
				wasPointerClicked = true;
			}
		}
	}
	
}
