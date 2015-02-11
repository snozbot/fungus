using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	
	public class MenuDialog : MonoBehaviour
	{
		protected Button[] cachedButtons;
		protected Slider cachedSlider;

		public virtual void Awake()
		{
			Button[] optionButtons = GetComponentsInChildren<Button>();
			cachedButtons = optionButtons;

			Slider timeoutSlider = GetComponentInChildren<Slider>();
			cachedSlider = timeoutSlider;

			Clear();
		}

		public virtual void OnEnable()
		{
			// The canvas may fail to update if the menu dialog is enabled in the first game frame.
			// To fix this we just need to force a canvas update when the object is enabled.
			Canvas.ForceUpdateCanvases();
		}

		protected virtual void Clear()
		{
			StopAllCoroutines();

			Button[] optionButtons = GetComponentsInChildren<Button>();						
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

			Slider timeoutSlider = GetComponentInChildren<Slider>();
			if (timeoutSlider != null)
			{
				timeoutSlider.gameObject.SetActive(false);
			}
		}

		public virtual bool AddOption(string text, Sequence targetSequence)
		{
			gameObject.SetActive(true);

			bool addedOption = false;
			foreach (Button button in cachedButtons)
			{
				if (!button.gameObject.activeSelf)
				{
					button.gameObject.SetActive(true);

					Text textComponent = button.GetComponentInChildren<Text>();
					if (textComponent != null)
					{
						textComponent.text = text;
					}
					
					Sequence sequence = targetSequence;
					
					button.onClick.AddListener(delegate {

						StopAllCoroutines(); // Stop timeout
						Clear();
						gameObject.SetActive(false);

						// Hide the active Say dialog in case it's still being displayed
						SayDialog activeSayDialog = SetSayDialog.GetActiveSayDialog();
						if (activeSayDialog != null)
						{
							activeSayDialog.ShowDialog(false);
						}

						if (sequence != null)
						{
							#if UNITY_EDITOR
							// Select the new target sequence in the Fungus Script window
							FungusScript fungusScript = sequence.GetFungusScript();
							fungusScript.selectedSequence = sequence;
							#endif

							sequence.ExecuteCommand(0);
						}
					});

					addedOption = true;
					break;
				}
			}
			
			return addedOption;
		}

		public virtual void ShowTimer(float duration, Sequence targetSequence)
		{
			gameObject.SetActive(true);

			if (cachedSlider != null)
			{
				cachedSlider.gameObject.SetActive(true);
				StopAllCoroutines();
				StartCoroutine(WaitForTimeout(duration, targetSequence));
			}
		}

		protected virtual IEnumerator WaitForTimeout(float timeoutDuration, Sequence targetSequence)
		{
			float elapsedTime = 0;
			
			Slider timeoutSlider = GetComponentInChildren<Slider>();
			
			while (elapsedTime < timeoutDuration)
			{
				if (timeoutSlider != null)
				{
					float t = 1f - elapsedTime / timeoutDuration;
					timeoutSlider.value = t;
				}
				
				elapsedTime += Time.deltaTime;
				
				yield return null;
			}
			
			Clear();
			gameObject.SetActive(false);
			
			if (targetSequence != null)
			{
				targetSequence.ExecuteCommand(0);
			}
		}
	}
	
}
