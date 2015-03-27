using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class SayDialog : Dialog 
	{
		// Currently active Say Dialog used to display Say text
		public static SayDialog activeSayDialog;

		public Image continueImage;
		public AudioClip continueSound;

		public static SayDialog GetSayDialog()
		{
			if (activeSayDialog == null)
			{
				// Use first Say Dialog found in the scene (if any)
				SayDialog sd = GameObject.FindObjectOfType<SayDialog>();
				if (sd != null)
				{
					activeSayDialog = sd;
				}
				
				if (activeSayDialog == null)
				{
					// Auto spawn a say dialog object from the prefab
					GameObject prefab = Resources.Load<GameObject>("SayDialog");
					if (prefab != null)
					{
						GameObject go = Instantiate(prefab) as GameObject;
						go.SetActive(false);
						go.name = "SayDialog";
						activeSayDialog = go.GetComponent<SayDialog>();
					}
				}
			}
			
			return activeSayDialog;
		}

		public virtual void Say(string text, bool waitForInput, Action onComplete)
		{
			Clear();

			Action onWritingComplete = delegate {
				if (waitForInput)
				{
					ShowContinueImage(true);
					StartCoroutine(WaitForInput(delegate {

						if (continueSound != null)
						{
							AudioSource.PlayClipAtPoint(continueSound, Vector3.zero);
						}
						Clear();
						StopVoiceOver();
						if (onComplete != null)
						{
							onComplete();
						}

					}));
				}
				else
				{
					if (onComplete != null)
					{
						onComplete();
					}
				}
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

		public override void Clear()
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
