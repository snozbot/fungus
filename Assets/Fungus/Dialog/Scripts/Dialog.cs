using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

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

		protected AudioSource voiceOverAudio;

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
		
		public virtual void SetCharacter(Character character, FungusScript fungusScript = null)
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

				if (fungusScript != null)
				{
					characterName = fungusScript.SubstituteVariables(characterName);
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

			// Parse the story text & tag markup to produce a list of tokens for processing
			DialogParser parser = new DialogParser();
			parser.Tokenize(text);

			if (parser.tokens.Count == 0)
			{
				if (onWritingComplete != null)
				{
					onWritingComplete();
				}
				yield break;
			}

			DialogText dialogText = new DialogText();
			dialogText.writingSpeed = writingSpeed;
			dialogText.punctuationPause = punctuationPause;

			GameObject typingAudio = null;
			if (characterTypingSound != null || writingSound != null)
			{
				typingAudio = new GameObject("WritingSound");
				typingAudio.AddComponent<AudioSource>();

				if (characterTypingSound != null)
				{
					typingAudio.audio.clip = characterTypingSound;
				}
				else if (writingSound != null)
				{
					typingAudio.audio.clip = writingSound;
				}

				typingAudio.audio.loop = loopWritingSound;
				typingAudio.audio.Play();

				dialogText.typingAudio = typingAudio.audio;
			}

			foreach (Token token in parser.tokens)
			{
				switch (token.type)
				{
				case TokenType.Words:
					dialogText.Append(token.param);
					break;

				case TokenType.BoldStart:
					dialogText.boldActive = true;
					break;

				case TokenType.BoldEnd:
					dialogText.boldActive = false;
					break;

				case TokenType.ItalicStart:
					dialogText.italicActive = true;
					break;

				case TokenType.ItalicEnd:
					dialogText.italicActive = false;
					break;

				case TokenType.ColorStart:
					dialogText.colorActive = true;
					dialogText.colorText = token.param;
					break;

				case TokenType.ColorEnd:
					dialogText.colorActive = false;
					break;

				case TokenType.Wait:
					float duration = 1f;
					if (!Single.TryParse(token.param, out duration))
					{
						duration = 1f;
					}
					yield return StartCoroutine(WaitForSecondsOrInput(duration));
					break;

				case TokenType.WaitForInputNoClear:
					OnWaitForInputTag(true);
					yield return StartCoroutine(WaitForInput(null));
					OnWaitForInputTag(false);
					break;
					
				case TokenType.WaitForInputAndClear:
					OnWaitForInputTag(true);
					yield return StartCoroutine(WaitForInput(null));
					OnWaitForInputTag(false);
					currentSpeed = writingSpeed;
					dialogText.Clear();
					StopVoiceOver();
					break;

				case TokenType.WaitOnPunctuation:
					float newPunctuationPause = 0f;
					if (!Single.TryParse(token.param, out newPunctuationPause))
					{
						newPunctuationPause = punctuationPause;
					}
					dialogText.punctuationPause = newPunctuationPause;
					break;

				case TokenType.Clear:
					dialogText.Clear();
					break;
					
				case TokenType.Speed:
					float newSpeed = 0;
					if (!Single.TryParse(token.param, out newSpeed))
					{
						newSpeed = 0f;
					}
					dialogText.writingSpeed = newSpeed;
					break;
					
				case TokenType.Exit:
					
					if (onExitTag != null)
					{
						Destroy(typingAudio);
						onExitTag();
					}
					
					yield break;

				case TokenType.Message:
					FungusScript.BroadcastFungusMessage(token.param);
					break;
				}

				// Update text writing
				while (!dialogText.UpdateGlyphs(wasPointerClicked))
				{
					storyText.text = dialogText.GetDialogText();
					yield return null;
				}
				storyText.text = dialogText.GetDialogText();
				wasPointerClicked = false;

				// Now process next token
			}

			Destroy(typingAudio);

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

		protected virtual IEnumerator WaitForSecondsOrInput(float duration)
		{
			float timer = duration;
			while (timer > 0 && !wasPointerClicked)
			{
				timer -= Time.deltaTime;
				yield return null;
			}
			
			wasPointerClicked = false;
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

		public virtual void PlayVoiceOver(AudioClip voiceOverSound)
		{
			if (voiceOverAudio == null)
			{
				voiceOverAudio = gameObject.AddComponent<AudioSource>();
			}
			voiceOverAudio.clip = voiceOverSound;
			voiceOverAudio.Play();
		}

		public virtual void StopVoiceOver()
		{
			if (voiceOverAudio)
			{
				Destroy(voiceOverAudio);
			}
		}
	}
	
}
