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

		public class Option
		{
			public string text;
			public Action onSelect;
		}

		public Canvas dialogCanvas;
		public List<UnityEngine.UI.Button> optionButtons = new List<UnityEngine.UI.Button>();
		public Text nameText;
		public Text storyText;
		public Image continueImage;
		public Image leftImage;
		public Image rightImage;

		List<Action> optionActions = new List<Action>();

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

		public void Say(string text, Action onComplete)
		{
			Clear();

			if (storyText != null)
			{
				storyText.text = text;
			}

			StartCoroutine(WriteText(text, delegate {
				ShowContinueIcon(true);
				StartCoroutine(WaitForInput(onComplete));
			}));
		}

		public void Say(string text, List<Option> options)
		{
			Clear();

			ShowContinueIcon(false);

			StartCoroutine(WriteText(text, delegate {
				foreach (Option option in options)
				{
					AddOption(option.text, option.onSelect);
				}
			}));
		}

		IEnumerator WriteText(string text, Action onWritingComplete)
		{
			if (storyText != null)
			{
				storyText.text = text;
			}

			if (onWritingComplete != null)
			{
				onWritingComplete();
			}

			yield break;
		}

		IEnumerator WaitForInput(Action onComplete)
		{
			// TODO: Handle touch input
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}

			Clear();

			if (onComplete != null)
			{
				onComplete();
			}
		}

		void ShowContinueIcon(bool visible)
		{
			if (continueImage != null)
			{
				continueImage.enabled = visible;
			}
		}

		void Clear()
		{
			ClearStoryText();
			ClearOptions();
		}

		void ClearStoryText()
		{
			if (storyText != null)
			{
				storyText.text = "";
			}
		}

		void ClearOptions()
		{
			if (optionButtons == null)
			{
				return;
			}

			optionActions.Clear();

			foreach (UnityEngine.UI.Button button in optionButtons)
			{
				if (button != null)
				{
					button.gameObject.SetActive(false);
				}
			}
		}

		bool AddOption(string text, Action action)
		{
			if (optionButtons == null)
			{
				return false;
			}

			bool addedOption = false;
			foreach (UnityEngine.UI.Button button in optionButtons)
			{
				if (!button.gameObject.activeSelf)
				{
					button.gameObject.SetActive(true);

					Text textComponent = button.GetComponentInChildren<Text>();
					if (textComponent != null)
					{
						textComponent.text = text;
					}

					optionActions.Add(action);

					addedOption = true;
					break;
				}
			}

			return addedOption;
		}

		public void SelectOption(int index)
		{
			if (index < optionActions.Count)
			{
				Action optionAction = optionActions[index];
				if (optionAction != null)
				{
					Clear();
					optionAction();
				}
			}
		}
	}

}
