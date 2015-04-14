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
		public static Character speakingCharacter;
		public static string prevStoryText;
		
		public float writingSpeed = 60;
		public AudioClip writingSound;
		public bool loopWritingSound = true;
		public bool beepPerCharacter = false;
		public float slowBeepsAt = 10f;
		public float fastBeepsAt = 30f;
		public float punctuationPause = 0.25f;
		public bool alwaysFadeDialog = false;
		public float fadeDuration = 1f;
		public LeanTweenType fadeEaseType;
		public bool alwaysMoveDialog = false;
		public Vector2 startPosition;
		protected Vector2 endPosition;
		public float moveSpeed = 1000f;
		public LeanTweenType moveEaseType;
		
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
				LeanTween.cancel(dialogCanvas.gameObject);
				CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
				if (canvasGroup != null)
				{
					canvasGroup.alpha = 1;
				}
				dialogCanvas.gameObject.SetActive(visible);
			}
			if (visible)
			{
				// A new dialog is often shown as the result of a mouse click, so we need
				// to make sure the previous click doesn't register on the new dialogue
				wasPointerClicked = false;
				clickCooldownTimer = 0.2f;
			}
		}
		
		public virtual void FadeInDialog()
		{
			LeanTween.cancel(dialogCanvas.gameObject);
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			if (canvasGroup != null)
			{
				canvasGroup.alpha = 0;
			}
			dialogCanvas.gameObject.SetActive(true);
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(dialogCanvas.gameObject,0,1,fadeDuration).setEase(fadeEaseType).setOnUpdate(
				(float fadeAmount)=>{
				if (canvasGroup != null)
				{
					canvasGroup.alpha = fadeAmount;
				}
			}
			).setOnComplete(
				()=>{
				if (canvasGroup != null)
				{
					canvasGroup.alpha = 1;
				}
			}
			);
		}

		public virtual void MoveInDialog()
		{
			endPosition = this.transform.position;
			float moveDuration = (Vector3.Distance(startPosition,this.transform.position)/moveSpeed);
			if (moveSpeed == 0) moveDuration = float.Epsilon;
			LeanTween.value(this.gameObject,(Vector2)startPosition,(Vector2)endPosition,moveDuration).setEase(moveEaseType).setOnUpdate(
				(Vector3 updatePosition)=>{
				this.transform.position = updatePosition;
			}
			).setOnComplete(
				()=>{
				this.transform.position = endPosition;
			}
			);
		}
		
		public virtual void FadeOutDialog()
		{
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			LeanTween.cancel(dialogCanvas.gameObject);
			if (fadeDuration == 0) fadeDuration = float.Epsilon;
			LeanTween.value(dialogCanvas.gameObject,1,0,fadeDuration).setEase(fadeEaseType).setOnUpdate(
				(float fadeAmount)=>{
				if (canvasGroup != null)
				{
					canvasGroup.alpha = fadeAmount;
				}
			}
			).setOnComplete(
				()=>{
				dialogCanvas.gameObject.SetActive(false);
				if (canvasGroup != null)
				{
					canvasGroup.alpha = 1;
				}
			}
			);
		}

		public virtual void MoveOutDialog()
		{
			endPosition = this.transform.position;
			float moveDuration = (Vector3.Distance(startPosition,this.transform.position)/moveSpeed);
			if (moveSpeed == 0) moveDuration = float.Epsilon;
			LeanTween.value(this.gameObject,(Vector2)endPosition,(Vector2)startPosition,moveDuration).setEase(moveEaseType).setOnUpdate(
				(Vector3 updatePosition)=>{
				this.transform.position = updatePosition;
			}
			).setOnComplete(
				()=>{
				this.transform.position = endPosition;
			}
			);
		}
		
		public virtual void SetCharacter(Character character, Flowchart flowchart = null)
		{
			if (character == null)
			{
				if (characterImage != null)
					characterImage.gameObject.SetActive(false);
				if (nameText != null)
					nameText.text = "";
				characterTypingSound = null;
			}
			else
			{
				Character prevSpeakingCharacter = speakingCharacter;
				speakingCharacter = character;
				
				// Dim portraits of non-speaking characters
				foreach (Stage s in Stage.activeStages)
				{
					if (s.dimPortraits)
					{
						foreach (Character c in s.charactersOnStage)
						{
							if (prevSpeakingCharacter != speakingCharacter)
							{
								if (c != speakingCharacter)
								{
									Portrait.Dim(c,s);
								}
								else
								{
									Portrait.Undim(c,s);
								}
							}
						}
					}
				}
				
				string characterName = character.nameText;

				if (characterName == "")
				{
					// Use game object name as default
					characterName = character.name;
				}
				
				if (flowchart != null)
				{
					characterName = flowchart.SubstituteVariables(characterName);
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
					characterImage.gameObject.SetActive(true);
				}
				else
				{
					characterImage.gameObject.SetActive(false);
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
			dialogText.beepPerCharacter = beepPerCharacter;
			dialogText.slowBeepsAt = slowBeepsAt;
			dialogText.fastBeepsAt = fastBeepsAt;
			
			GameObject typingAudio = null;
			if (characterTypingSound != null || writingSound != null)
			{
				typingAudio = new GameObject("WritingSound");
				typingAudio.AddComponent<AudioSource>();
				typingAudio.hideFlags = HideFlags.HideInHierarchy;
				
				if (characterTypingSound != null)
				{
					typingAudio.GetComponent<AudioSource>().clip = characterTypingSound;
				}
				else if (writingSound != null)
				{
					typingAudio.GetComponent<AudioSource>().clip = writingSound;
				}

				typingAudio.GetComponent<AudioSource>().loop = loopWritingSound;
				typingAudio.GetComponent<AudioSource>().Play();

				dialogText.typingAudio = typingAudio.GetComponent<AudioSource>();
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
					
				case TokenType.WaitOnPunctuationStart:
					float newPunctuationPause = 0f;
					if (!Single.TryParse(token.param, out newPunctuationPause))
					{
						newPunctuationPause = 0f;
					}
					dialogText.punctuationPause = newPunctuationPause;
					break;
				case TokenType.WaitOnPunctuationEnd:
					dialogText.punctuationPause = punctuationPause;
					break;
					
				case TokenType.Clear:
					dialogText.Clear();
					break;
					
				case TokenType.SpeedStart:
					float newSpeed = 0;
					if (!Single.TryParse(token.param, out newSpeed))
					{
						newSpeed = 0f;
					}
					dialogText.writingSpeed = newSpeed;
					break;
					
				case TokenType.SpeedEnd:
					dialogText.writingSpeed = writingSpeed;
					break;
					
				case TokenType.Exit:
					if (onExitTag != null)
					{
						prevStoryText = storyText.text;
						Destroy(typingAudio);
						onExitTag();
					}
					yield break;
					
				case TokenType.Message:
					Flowchart.BroadcastFungusMessage(token.param);
					break;
				case TokenType.VerticalPunch:
					float vPunchIntensity = 0;
					if (!Single.TryParse(token.param, out vPunchIntensity))
					{
						vPunchIntensity = 0f;
					}
					VerticalPunch(vPunchIntensity);
					break;
				case TokenType.HorizontalPunch:
					float hPunchIntensity = 0;
					if (!Single.TryParse(token.param, out hPunchIntensity))
					{
						hPunchIntensity = 0f;
					}
					HorizontalPunch(hPunchIntensity);
					break;
				case TokenType.Shake:
					float shakeIntensity = 0;
					if (!Single.TryParse(token.param, out shakeIntensity))
					{
						shakeIntensity = 0f;
					}
					Shake(shakeIntensity);
					break;
				case TokenType.Shiver:
					float shiverIntensity = 0;
					if (!Single.TryParse(token.param, out shiverIntensity))
					{
						shiverIntensity = 0f;
					}
					Shiver(shiverIntensity);
					break;
				case TokenType.Flash:
					float flashDuration = 0;
					if (!Single.TryParse(token.param, out flashDuration))
					{
						flashDuration = 0f;
					}
					Flash(flashDuration);
					break;
				case TokenType.Audio:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.PlayOneShot(audioSource.clip);
						}
					}
					break;
				case TokenType.AudioLoop:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Play();
							audioSource.loop = true;
						}
					}
					break;
				case TokenType.AudioPause:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Pause ();
						}
					}
					break;
				case TokenType.AudioStop:
					{
						AudioSource audioSource = FindAudio(token.param);
						if (audioSource != null)
						{
							audioSource.Pause ();
						}
					}
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

			prevStoryText = storyText.text;
			
			Destroy(typingAudio);

			if (onWritingComplete != null)
			{
				onWritingComplete();
			}
			
			yield break;
		}

		protected virtual AudioSource FindAudio(string audioObjectName)
		{
			GameObject go = GameObject.Find(audioObjectName);
			if (go == null)
			{
				return null;
			}

			return go.GetComponent<AudioSource>();
		}

		protected virtual void VerticalPunch(float intensity)
		{
			iTween.ShakePosition(this.gameObject, new Vector3(0f, intensity, 0f), 0.5f);
		}
		
		protected virtual void HorizontalPunch(float intensity)
		{
			iTween.ShakePosition(this.gameObject, new Vector3(intensity, 0f, 0f), 0.5f);
		}
		
		protected virtual void Shake(float intensity)
		{
			iTween.ShakePosition(this.gameObject, new Vector3(intensity, intensity, 0f), 0.5f);
		}
		
		protected virtual void Shiver(float intensity)
		{
			iTween.ShakePosition(this.gameObject, new Vector3(intensity, intensity, 0f), 1f);
		}
		
		protected virtual void Flash(float duration)
		{
			CameraController cameraController = CameraController.GetInstance();
			cameraController.screenFadeTexture = CameraController.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
			cameraController.Fade(1f, duration, delegate {
				cameraController.screenFadeTexture = CameraController.CreateColorTexture(new Color(1f,1f,1f,1f), 32, 32);
				cameraController.Fade(0f, duration, null);
			});
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
				// Stop all tweening portraits
				foreach( Character c in Character.activeCharacters )
				{
					if (c.state.portraitImage != null)
					{
						if (LeanTween.isTweening(c.state.portraitObj))
						{
							LeanTween.cancel(c.state.portraitObj, true);
							c.state.portraitImage.material.SetFloat( "_Fade", 1 );
							Portrait.SetRectTransform(c.state.portraitImage.rectTransform, c.state.position);
							if (c.state.dimmed == true)
							{
								c.state.portraitImage.material.SetColor("_Color", new Color(0.5f,0.5f,0.5f,1f));
							}
							else
							{
								c.state.portraitImage.material.SetColor("_Color", new Color(1f,1f,1f,1f));
							}
						}
					}
				}
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
