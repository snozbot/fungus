using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class SayDialog : MonoBehaviour, IDialogInputListener 
	{
		// Currently active Say Dialog used to display Say text
		public static SayDialog activeSayDialog;

		public static Character speakingCharacter;

		public Image continueImage;
		public AudioClip continueSound;
		public bool visibleAtStart;
		public float fadeDuration = 1f;
		
		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image characterImage;
	
		public DialogAudio audioController = new DialogAudio();
		
		protected Writer writer;
		protected bool wasPointerClicked;

		public static SayDialog GetSayDialog()
		{
			if (activeSayDialog == null)
			{
				// Use first Say Dialog found in the scene (if any)
				SayDialog sd = GameObject.FindObjectOfType<SayDialog>();
				if (sd != null)
				{
					activeSayDialog = sd;
				}
				
				if (activeSayDialog == null)
				{
					// Auto spawn a say dialog object from the prefab
					GameObject prefab = Resources.Load<GameObject>("SayDialog");
					if (prefab != null)
					{
						GameObject go = Instantiate(prefab) as GameObject;
						go.SetActive(false);
						go.name = "SayDialog";
						activeSayDialog = go.GetComponent<SayDialog>();
						activeSayDialog.visibleAtStart = true;
					}
				}
			}
			
			return activeSayDialog;
		}

		protected Writer GetWriter()
		{
			if (writer != null)
			{
				return writer;
			}

			writer = GetComponent<Writer>();
			if (writer == null)
			{
				writer = gameObject.AddComponent<Writer>();
			}

			return writer;
		}

		protected virtual void Start()
		{
			// Set dialog visibilty at startup
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			if (visibleAtStart)
			{
				canvasGroup.alpha = 1f;
			}
			else
			{
				canvasGroup.alpha = 0f;
			}
		}

		public virtual void Say(string text, bool clearPrevious, bool waitForInput, AudioClip voiceOverClip, Action onComplete)
		{
			GetWriter().Write(text, clearPrevious, waitForInput, onComplete);
		}

		protected virtual void Update()
		{
			if (continueImage != null)
			{
				continueImage.enabled = GetWriter().isWaitingForInput;
			}
		}

		protected virtual void LateUpdate()
		{
			wasPointerClicked = false;
		}
		
		public virtual void ShowDialog(bool visible)
		{
			gameObject.SetActive(true);
			
			if (visible)
			{
				// A new dialog is often shown as the result of a mouse click, so we need
				// to make sure the previous click doesn't register on the new dialogue
				wasPointerClicked = false;
			}
		}
		
		public virtual void SetCharacter(Character character, Flowchart flowchart = null)
		{
			if (character == null)
			{
				if (characterImage != null)
				{
					characterImage.gameObject.SetActive(false);
				}
				if (nameText != null)
				{
					nameText.text = "";
				}
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
