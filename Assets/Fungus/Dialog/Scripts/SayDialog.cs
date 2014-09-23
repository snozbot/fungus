using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[ExecuteInEditMode]
	public class SayDialog : Dialog 
	{
		public Image continueImage;

		static public List<SayDialog> activeDialogs = new List<SayDialog>();

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

		public virtual void Say(string text, Action onComplete)
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

		protected virtual void ShowContinueImage(bool visible)
		{
			if (continueImage != null)
			{
				continueImage.enabled = visible;
			}
		}
	}

}
