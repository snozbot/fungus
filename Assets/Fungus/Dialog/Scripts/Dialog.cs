using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Fungus.Script
{
	
	public class Dialog : MonoBehaviour 
	{
		public enum DialogSide
		{
			Left,
			Right
		};

		public float writingSpeed;
		public AudioClip writingSound;
		public bool loopWritingSound = true;
		
		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image leftImage;
		public Image rightImage;

		protected enum GlyphType
		{
			Character,				// Text character
			Wait, 					// w, w=0.5
			WaitForInput, 			// i
			WaitForInputAndClear, 	// ic
			Clear, 					// c
			Speed, 					// s, s=60
			Exit, 					// x
			Punctuation 			// p, p=0.5
		}

		protected class Glyph
		{
			public GlyphType type = GlyphType.Character;
			public string param = "";
		}

		public void ShowDialog(bool visible)
		{
			if (dialogCanvas != null)
			{
				dialogCanvas.gameObject.SetActive(visible);
			}
		}
		
		public void SetCharacter(Character character)
		{
			if (character == null)
			{
				if (leftImage != null)
					leftImage.enabled = false;
				if (rightImage != null)
					rightImage.enabled = false;
				if (nameText != null)
					nameText.text = "";
			}
			else
			{
				SetCharacterImage(character.characterImage, character.dialogSide);
				SetCharacterName(character.name, character.characterColor);
			}
		}
		
		public void SetCharacterImage(Sprite image, DialogSide side)
		{
			if (leftImage != null)
			{
				if (image != null &&
				    side == DialogSide.Left)
				{
					leftImage.sprite = image;
					leftImage.enabled = true;
				}
				else
				{
					leftImage.enabled = false;
				}
			}
			
			if (rightImage != null)
			{
				if (image != null &&
				    side == DialogSide.Right)
				{
					rightImage.sprite = image;
					rightImage.enabled = true;
				}
				else
				{
					rightImage.enabled = false;
				}
			}
		}
		
		public void SetCharacterName(string name, Color color)
		{
			if (nameText != null)
			{
				nameText.text = name;
				nameText.color = color;
			}
		}

		protected IEnumerator WriteText(string text, Action onWritingComplete, Action onExitTag)
		{
			storyText.text = "";

			List<Glyph> glyphs = MakeGlyphList(text);

			if (glyphs.Count == 0)
			{
				yield break;
			}

			// Zero writingSpeed means write instantly
			// Also write instantly if text contains rich text markup tags
			if (writingSpeed == 0 ||
			    text.Contains("<"))
			{
				foreach (Glyph glyph in glyphs)
				{
					if (glyph.type == GlyphType.Character)
					{
						storyText.text += glyph.param;
					}
				}

				if (onWritingComplete != null)
				{
					onWritingComplete();
				}
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
			
			float writeDelay = 0f;
			if (writingSpeed > 0)
			{
				writeDelay = (1f / (float)writingSpeed);
			}

			float timeAccumulator = 0f;

			int i = 0;
			while (i < glyphs.Count)
			{
				timeAccumulator += Time.deltaTime;
				
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
							storyText.text += glyph.param;
						}
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

					case GlyphType.WaitForInput:
						OnWaitForInputTag(true);
						yield return StartCoroutine(WaitForInput(null));
						OnWaitForInputTag(false);
						break;
					
					case GlyphType.WaitForInputAndClear:
						OnWaitForInputTag(true);
						yield return StartCoroutine(WaitForInput(null));
						OnWaitForInputTag(false);
						storyText.text = "";
						break;

					case GlyphType.Clear:
						storyText.text = "";
						break;

					case GlyphType.Speed:
						if (!Single.TryParse(glyph.param, out writingSpeed))
						{
							writingSpeed = 0f;
						}
						writeDelay = 0;
						if (writingSpeed > 0)
						{
							writeDelay = (1f / (float)writingSpeed);
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

					case GlyphType.Punctuation:
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
		
		void ClearStoryText()
		{
			if (storyText != null)
			{
				storyText.text = "";
			}
		}

		List<Glyph> MakeGlyphList(string storyText)
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
					glyphList.Add(MakeCharacterGlyph(c.ToString()));
				}

				Glyph tagGlyph = MakeTagGlyph(tagText);
				if (tagGlyph != null)
				{
					glyphList.Add (tagGlyph);
				}

				position = m.Index + tagText.Length;
				m = m.NextMatch();
			}
			
			if (position < storyText.Length - 1)
			{
				string postText = storyText.Substring(position, storyText.Length - position);
				foreach (char c in postText)
				{
					glyphList.Add(MakeCharacterGlyph(c.ToString()));
				}
			}

			return glyphList;
		}

		Glyph MakeCharacterGlyph(string character)
		{
			Glyph glyph = new Glyph();
			glyph.type = GlyphType.Character;
			glyph.param = character;
			return glyph;
		}

		Glyph MakeTagGlyph(string tagText)
		{
			if (tagText.Length < 3 ||
			    tagText.Substring(0,1) != "{" ||
			    tagText.Substring(tagText.Length - 1,1) != "}")
			{
				return null;
			}

			string tag = tagText.Substring(1, tagText.Length - 2);

			GlyphType type = GlyphType.Character;
			string paramText = "";

			if (tag == "i")
			{
				type = GlyphType.WaitForInput;
			}
			if (tag == "ic")
			{
				type = GlyphType.WaitForInputAndClear;
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
			else if (tag.StartsWith("p="))
			{
				type = GlyphType.Punctuation;
				paramText = tag.Substring(2, tag.Length - 2);
			}
			else if (tag == "p")
			{
				type = GlyphType.Punctuation;
			}

			Glyph glyph = new Glyph();
			glyph.type = type;
			glyph.param = paramText;

			return glyph;
		}

		protected IEnumerator WaitForInput(Action onInput)
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
