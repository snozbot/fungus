using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class Dialog : MonoBehaviour, IDialogInputListener
	{
		public static Character speakingCharacter;

		public float writingSpeed = 60;
		public AudioClip writingSound;
		[Range(0,1)]
		public float writingVolume = 1f;
		public bool loopWritingSound = true;
		public bool beepPerCharacter = false;
		public float slowBeepsAt = 10f;
		public float fastBeepsAt = 30f;
		public float punctuationPause = 0.25f;
		public bool alwaysFadeDialog = false;
		public float fadeDuration = 1f;
		public LeanTweenType fadeEaseType;

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

		protected bool wasPointerClicked;

		public DialogAudio audioController = new DialogAudio();

		protected virtual void LateUpdate()
		{
			wasPointerClicked = false;
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

			if (fadeDuration == 0)
			{
				fadeDuration = float.Epsilon;
			}

			LeanTween.value(dialogCanvas.gameObject,0,1,fadeDuration).setEase(fadeEaseType).setOnUpdate( (float fadeAmount)=> {
				if (canvasGroup != null)
				{
					canvasGroup.alpha = fadeAmount;
				}
			}).setOnComplete( ()=> {
				if (canvasGroup != null)
				{
					canvasGroup.alpha = 1;
				}
			});
		}

		public virtual void FadeOutDialog()
		{
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			LeanTween.cancel(dialogCanvas.gameObject);
			if (fadeDuration == 0) 
			{	
				fadeDuration = float.Epsilon;
			}

			LeanTween.value(dialogCanvas.gameObject,1,0,fadeDuration).setEase(fadeEaseType).setOnUpdate( (float fadeAmount)=> {
				if (canvasGroup != null)
				{
					canvasGroup.alpha = fadeAmount;
				}
			}).setOnComplete( ()=> {
				dialogCanvas.gameObject.SetActive(false);
				if (canvasGroup != null)
				{
					canvasGroup.alpha = 1;
				}
			});
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
									Portrait.SetDimmed(c, s, true);
								}
								else
								{
									Portrait.SetDimmed(c, s, false);
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

		public static void StopPortraitTweens()
		{
			// Stop all tweening portraits
			foreach( Character c in Character.activeCharacters )
			{
				if (c.state.portraitImage != null)
				{
					if (LeanTween.isTweening(c.state.portraitImage.gameObject))
					{
						LeanTween.cancel(c.state.portraitImage.gameObject, true);

						Portrait.SetRectTransform(c.state.portraitImage.rectTransform, c.state.position);
						if (c.state.dimmed == true)
						{
							c.state.portraitImage.color = new Color(0.5f, 0.5f, 0.5f, 1f);
						}
						else
						{
							c.state.portraitImage.color = Color.white;
						}
					}
				}
			}
		}

		//
		// IDialogInput implementation
		//

		public virtual void OnNextLineEvent()
		{
			wasPointerClicked = true;
		}

	}
	
}
