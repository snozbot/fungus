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
		
		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image characterImage;

		protected float currentSpeed;
		protected float currentPunctuationPause;
		protected bool boldActive;
		protected bool italicActive;
		protected bool colorActive;
		protected string colorText;

		protected enum GlyphType
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

		protected class Glyph
		{
			public GlyphType type = GlyphType.Character;
			public string param = "";
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

			List<Glyph> glyphs = MakeGlyphList(text);

			if (glyphs.Count == 0)
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
				typingAudio.audio.clip = writingSound;
				typingAudio.audio.loop = loopWritingSound;
				typingAudio.audio.Play();
			}

			float timeAccumulator = 0f;

			int i = 0;
			while (i < glyphs.Count)
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

					Glyph glyph = glyphs[i];

					switch (glyph.type)
					{
					case GlyphType.Character:

						if (storyText.text.Length == 0 && glyph.param == "\n")
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
			
							storyText.text += start + glyph.param + end;

							if (Input.GetMouseButtonDown(0))
							{
								currentSpeed = 10000; // Write instantly
							}
						}

						// Add a wait glyph on punctuation marks
						bool doPause = punctuationPause > 0 && IsPunctuation(glyph.param);
						if (i == glyphs.Count - 1)
						{
							doPause = false; // No pause on last character
						}
						else 
						{
							// No pause if next glyph is a pause
							GlyphType nextType = glyphs[i + 1].type;
							if (nextType == GlyphType.Wait ||
							    nextType == GlyphType.WaitForInputAndClear ||
							    nextType == GlyphType.WaitForInputNoClear)
							{
								doPause = false;
							}
						}

						if (doPause)
						{
							// Ignore if next glyph is also punctuation, or if punctuation is the last character.
							bool skipCharacter = (i < glyphs.Count - 1 &&
							             glyphs[i + 1].type == GlyphType.Character &&
							             IsPunctuation(glyphs[i + 1].param));

							if (!skipCharacter)
							{
								yield return new WaitForSeconds(currentPunctuationPause);
							}
						}

						break;

					case GlyphType.BoldStart:
						boldActive = true;
						break;
						
					case GlyphType.BoldEnd:
						boldActive = false;
						break;
					
					case GlyphType.ItalicStart:
						italicActive = true;
						break;
						
					case GlyphType.ItalicEnd:
						italicActive = false;
						break;

					case GlyphType.ColorStart:
						colorActive = true;
						colorText = glyph.param;
						break;
						
					case GlyphType.ColorEnd:
						colorActive = false;
						break;

					case GlyphType.Wait:
						float duration = 1f;
						if (!Single.TryParse(glyph.param, out duration))
						{
							duration = 1f;
						}
						yield return new WaitForSeconds(duration);
						timeAccumulator = 0f;
						break;

					case GlyphType.WaitForInputNoClear:
						OnWaitForInputTag(true);
						yield return StartCoroutine(WaitForInput(null));
						timeAccumulator = 0f;
						OnWaitForInputTag(false);
						break;
					
					case GlyphType.WaitForInputAndClear:
						OnWaitForInputTag(true);
						yield return StartCoroutine(WaitForInput(null));
						OnWaitForInputTag(false);
						timeAccumulator = 0f;
						storyText.text = "";
						break;

					case GlyphType.Clear:
						storyText.text = "";
						timeAccumulator = 0f;
						break;

					case GlyphType.Speed:
						if (!Single.TryParse(glyph.param, out currentSpeed))
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

					case GlyphType.Exit:

						if (typingAudio != null)
						{
							Destroy(typingAudio);
						}

						if (onExitTag != null)
						{
							onExitTag();
						}

						yield break;

					case GlyphType.WaitOnPunctuation:
						if (!Single.TryParse(glyph.param, out currentPunctuationPause))
						{
							currentPunctuationPause = 0f;
						}
						break;
					}

					if (++i >= glyphs.Count)
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

		protected virtual void Clear()
		{
			ClearStoryText();
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
		
		protected virtual List<Glyph> MakeGlyphList(string storyText)
		{
			List<Glyph> glyphList = new List<Glyph>();

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
					AddCharacterGlyph(glyphList, c.ToString());
				}

				AddTagGlyph(glyphList, tagText);

				position = m.Index + tagText.Length;
				m = m.NextMatch();
			}
			
			if (position < storyText.Length - 1)
			{
				string postText = storyText.Substring(position, storyText.Length - position);
				foreach (char c in postText)
				{
					AddCharacterGlyph(glyphList, c.ToString());
				}
			}

			return glyphList;
		}

		protected virtual void AddCharacterGlyph(List<Glyph> glyphList, string character)
		{
			Glyph glyph = new Glyph();
			glyph.type = GlyphType.Character;
			glyph.param = character;
			glyphList.Add(glyph);
		}

		protected virtual void AddTagGlyph(List<Glyph> glyphList, string tagText)
		{
			if (tagText.Length < 3 ||
			    tagText.Substring(0,1) != "{" ||
			    tagText.Substring(tagText.Length - 1,1) != "}")
			{
				return;
			}

			string tag = tagText.Substring(1, tagText.Length - 2);

			GlyphType type = GlyphType.Character;
			string paramText = "";

			if (tag == "b")
			{
				type = GlyphType.BoldStart;
			}
			else if (tag == "/b")
			{
				type = GlyphType.BoldEnd;
			}
			else if (tag == "i")
			{
				type = GlyphType.ItalicStart;
			}
			else if (tag == "/i")
			{
				type = GlyphType.ItalicEnd;
			}
			else if (tag.StartsWith("color="))
			{
				type = GlyphType.ColorStart;
				paramText = tag.Substring(6, tag.Length - 6);
			}
			else if (tag == "/color")
			{
				type = GlyphType.ColorEnd;
			}
			else if (tag == "wi")
			{
				type = GlyphType.WaitForInputNoClear;
			}
			if (tag == "wc")
			{
				type = GlyphType.WaitForInputAndClear;
			}
			else if (tag.StartsWith("wp="))
			{
				type = GlyphType.WaitOnPunctuation;
				paramText = tag.Substring(3, tag.Length - 3);
			}
			else if (tag == "wp")
			{
				type = GlyphType.WaitOnPunctuation;
			}
			else if (tag.StartsWith("w="))
			{
				type = GlyphType.Wait;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "w")
			{
				type = GlyphType.Wait;
			}
			else if (tag == "c")
			{
				type = GlyphType.Clear;
			}
			else if (tag.StartsWith("s="))
			{
				type = GlyphType.Speed;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "s")
			{
				type = GlyphType.Speed;
			}
			else if (tag == "x")
			{
				type = GlyphType.Exit;
			}

			Glyph glyph = new Glyph();
			glyph.type = type;
			glyph.param = paramText.Trim();

			glyphList.Add(glyph);
		}

		protected virtual IEnumerator WaitForInput(Action onInput)
		{
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}

			if (onInput != null)
			{
				onInput();
			}
		}

		protected virtual void OnWaitForInputTag(bool waiting)
		{}
	}
	
}
