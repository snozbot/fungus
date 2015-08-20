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

		protected virtual void Start()
		{
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
		}

		public virtual void Say(string text, bool clearPrevious, bool waitForInput, AudioClip voiceOverClip, Action onComplete)
		{
			Writer writer = GetComponent<Writer>();
			if (writer == null)
			{
				writer = gameObject.AddComponent<Writer>();
			}

			ShowContinueImage(false);

			writer.Write(text, clearPrevious, waitForInput, onComplete);
		}

		public override void Clear()
		{
			base.Clear();
			ShowContinueImage(false);
		}

		protected virtual void OnWaitForInputTag(bool waiting)
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
