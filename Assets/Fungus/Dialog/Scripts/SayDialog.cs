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

			Action onWritingComplete = delegate {
				ShowContinueImage(true);
				StartCoroutine(WaitForInput(delegate {
					Clear();					
					if (onComplete != null)
					{
						onComplete();
					}
				}));
			};

			Action onExitTag = delegate {
				Clear();					
				if (onComplete != null)
				{
					onComplete();
				}
			};

			StartCoroutine(WriteText(text, onWritingComplete, onExitTag));
		}

		protected override void Clear()
		{
			base.Clear();
			ShowContinueImage(false);
		}

		protected override void OnWaitForInputTag(bool waiting)
		{
			ShowContinueImage(waiting);
		}

		void ShowContinueImage(bool visible)
		{
			if (continueImage != null)
			{
				continueImage.enabled = visible;
			}
		}
	}

}
