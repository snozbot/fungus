using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Fungus.Script
{

	public class SayDialog : Dialog 
	{
		public void Say(string text, Action onComplete)
		{
			Clear();
			
			if (storyText != null)
			{
				storyText.text = text;
			}
			
			StartCoroutine(WriteText(text, delegate {
								
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
		}

		IEnumerator WaitForInput(Action onInput)
		{
			// TODO: Handle touch input
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
