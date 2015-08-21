using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class SayDialog : MonoBehaviour
	{
		// Currently active Say Dialog used to display Say text
		public static SayDialog activeSayDialog;

		public static Character speakingCharacter;

		public Image continueImage;
		public AudioClip continueSound;
		public float fadeDuration = 0.25f;
		
		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image characterImage;
	
		public DialogAudio audioController = new DialogAudio();
		
		protected Writer writer;
		protected CanvasGroup canvasGroup;
		protected bool fadeWhenDone = true;
		protected float targetAlpha = 0f;
		protected float fadeCoolDownTimer = 0f;

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

		protected CanvasGroup GetCanvasGroup()
		{
			if (canvasGroup != null)
			{
				return canvasGroup;
			}
			
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null)
			{
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			}
			
			return canvasGroup;
		}

		protected void Start()
		{
			// Dialog always starts invisible, will be faded in when writing starts
			GetCanvasGroup().alpha = 0f;
		}

		public virtual void Say(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, AudioClip voiceOverClip, Action onComplete)
		{
			this.fadeWhenDone = fadeWhenDone;

			GetWriter().Write(text, clearPrevious, waitForInput, onComplete);
		}

		protected virtual void LateUpdate()
		{
			UpdateAlpha();

			if (continueImage != null)
			{
				continueImage.enabled = GetWriter().isWaitingForInput;
			}
		}

		public virtual void FadeOut()
		{
			fadeWhenDone = true;
		}

		protected virtual void UpdateAlpha()
		{
			if (GetWriter().isWriting)
			{
				targetAlpha = 1f;
				fadeCoolDownTimer = 0.1f;
			}
			else if (fadeWhenDone && fadeCoolDownTimer == 0f)
			{
				targetAlpha = 0f;
			}
			else
			{
				// Add a short delay before we start fading in case there's another Say command in the next frame or two.
				// This avoids a noticeable flicker between consecutive Say commands.
				fadeCoolDownTimer = Mathf.Max(0f, fadeCoolDownTimer - Time.deltaTime);
			}

			CanvasGroup canvasGroup = GetCanvasGroup();
			float fadeDuration = GetSayDialog().fadeDuration;
			if (fadeDuration <= 0f)
			{
				canvasGroup.alpha = targetAlpha;
			}
			else
			{
				float delta = (1f / fadeDuration) * Time.deltaTime;
				float alpha = Mathf.MoveTowards(canvasGroup.alpha, targetAlpha, delta);
				canvasGroup.alpha = alpha;
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
	}

}
