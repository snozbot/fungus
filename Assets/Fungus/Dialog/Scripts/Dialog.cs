using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

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

		protected IEnumerator WriteText(string text, Action onWritingComplete)
		{
			// Zero CPS means write instantly
			// Also write instantly if text contains markup tags
			if (writingSpeed == 0 ||
			    text.Contains("<"))
			{
				storyText.text = text;
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
			
			storyText.text = "";
			
			// Make one character visible at a time
			float writeDelay = (1f / (float)writingSpeed);
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
				
				if (i >= text.Length)
				{
					storyText.text = text;
					break;
				}
				else
				{
					string left = text.Substring(0, i + 1);
					storyText.text = left;
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
		
	}
	
}
