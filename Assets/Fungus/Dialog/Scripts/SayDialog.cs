using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Fungus.Script
{

	public class SayDialog : Dialog 
	{
		public Image continueImage;

		public void Say(string text, Action onComplete)
		{
			Clear();
			
			if (storyText != null)
			{
				storyText.text = text;
			}
			
			StartCoroutine(WriteText(text, delegate {
				ShowContinueImage(true);
				
				StartCoroutine(WaitForInput(delegate {
					Clear();
					
					if (onComplete != null)
					{
						onComplete();
					}
				}));
				
			}));
		}

		protected override void Clear()
		{
			base.Clear();
			ShowContinueImage(false);
		}

		void ShowContinueImage(bool visible)
		{
			if (continueImage != null)
			{
				continueImage.enabled = visible;
			}
		}

		IEnumerator WaitForInput(Action onInput)
		{
			while (!Input.GetMouseButtonDown(0))
			{
				yield return null;
			}
			
			if (onInput != null)
			{
				onInput();
			}
		}
	}

}
