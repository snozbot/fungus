using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class ChooseDialog : Dialog 
	{
		public Slider slider;

		public class Option
		{
			public string text;
			public UnityAction onSelect;
		}

		public List<UnityEngine.UI.Button> optionButtons = new List<UnityEngine.UI.Button>();

		public void Choose(string text, List<Option> options, float timeoutDuration, Action onTimeout)
		{
			Clear();
			
			StartCoroutine(WriteText(text, delegate {
				foreach (Option option in options)
				{
					AddOption(option.text, option.onSelect);
				}
				
				if (timeoutDuration > 0)
				{
					StartCoroutine(WaitForTimeout(timeoutDuration, onTimeout));
				}
			}));
		}

		IEnumerator WaitForTimeout(float timeoutDuration, Action onTimeout)
		{
			float elapsedTime = 0;

			while (elapsedTime < timeoutDuration)
			{
				if (slider != null)
				{
					float t = elapsedTime / timeoutDuration;
					slider.value = t;
				}

				elapsedTime += Time.deltaTime;

				yield return null;
			}

			Clear();
			
			if (onTimeout != null)
			{
				onTimeout();
			}
		}

		protected override void Clear()
		{
			base.Clear();
			ClearOptions();
		}

		void ClearOptions()
		{
			if (optionButtons == null)
			{
				return;
			}

			foreach (UnityEngine.UI.Button button in optionButtons)
			{
				button.onClick.RemoveAllListeners();
			}

			foreach (UnityEngine.UI.Button button in optionButtons)
			{
				if (button != null)
				{
					button.gameObject.SetActive(false);
				}
			}
		}
		
		bool AddOption(string text, UnityAction action)
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

					UnityAction buttonAction = action;

					button.onClick.AddListener(delegate {
						StopAllCoroutines(); // Stop timeout
						Clear();
						if (buttonAction != null)
						{
							buttonAction();
						}
					});
					
					addedOption = true;
					break;
				}
			}
			
			return addedOption;
		}		
	}

}
