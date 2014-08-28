using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class ChooseDialog : Dialog 
	{
		public class Option
		{
			public string text;
			public Action onSelect;
		}

		public List<UnityEngine.UI.Button> optionButtons = new List<UnityEngine.UI.Button>();

		List<Action> optionActions = new List<Action>();

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
			yield return new WaitForSeconds(timeoutDuration);
			
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
					StopCoroutine("WaitForTimeout");
					Clear();
					optionAction();
				}
			}
		}
	}

}
