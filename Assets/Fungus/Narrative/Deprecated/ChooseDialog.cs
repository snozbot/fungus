using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class ChooseDialog : Dialog 
	{
		public Slider timeoutSlider;

		public class Option
		{
			public string text;
			public UnityAction onSelect;
		}

		public List<UnityEngine.UI.Button> optionButtons = new List<UnityEngine.UI.Button>();

		static public List<ChooseDialog> activeDialogs = new List<ChooseDialog>();

		protected virtual void OnEnable()
		{
			if (!activeDialogs.Contains(this))
			{
				activeDialogs.Add(this);
			}
		}
		
		protected virtual void OnDisable()
		{
			activeDialogs.Remove(this);
		}

		public override void ShowDialog (bool visible)
		{
			base.ShowDialog (visible);
			timeoutSlider.gameObject.SetActive(false);
		}

		public virtual void Choose(string text, List<Option> options, float timeoutDuration, Action onTimeout)
		{
			Clear();

			Action onWritingComplete = delegate {
				foreach (Option option in options)
				{
					AddOption(option.text, option.onSelect);
				}
				
				if (timeoutDuration > 0)
				{
					timeoutSlider.gameObject.SetActive(true);
					StartCoroutine(WaitForTimeout(timeoutDuration, onTimeout));
				}
			};

			StartCoroutine(WriteText(text, onWritingComplete, onTimeout));
		}

		protected virtual IEnumerator WaitForTimeout(float timeoutDuration, Action onTimeout)
		{
			float elapsedTime = 0;

			while (elapsedTime < timeoutDuration)
			{
				if (timeoutSlider != null)
				{
					float t = elapsedTime / timeoutDuration;
					timeoutSlider.value = t;
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

		public override void Clear()
		{
			base.Clear();
			ClearOptions();
		}

		protected virtual void ClearOptions()
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
		
		protected virtual bool AddOption(string text, UnityAction action)
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
