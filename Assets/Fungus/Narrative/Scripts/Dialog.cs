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

		public DialogAudio audioController = new DialogAudio();

		public float fadeDuration = 1f;

		public Canvas dialogCanvas;
		public Text nameText;
		public Text storyText;
		public Image characterImage;

		protected bool wasPointerClicked;

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
