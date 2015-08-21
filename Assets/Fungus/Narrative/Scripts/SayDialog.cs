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
		public bool visibleAtStart;

		protected Writer writer;

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
						activeSayDialog.visibleAtStart = true;
					}
				}
			}
			
			return activeSayDialog;
		}

		protected Writer GetWriter()
		{
			if (writer != null)
			{
				return writer;
			}

			writer = GetComponent<Writer>();
			if (writer == null)
			{
				writer = gameObject.AddComponent<Writer>();
			}

			return writer;
		}

		protected virtual void Start()
		{
			// Set dialog visibilty at startup
			CanvasGroup canvasGroup = dialogCanvas.GetComponent<CanvasGroup>();
			if (visibleAtStart)
			{
				canvasGroup.alpha = 1f;
			}
			else
			{
				canvasGroup.alpha = 0f;
			}
		}

		public virtual void Say(string text, bool clearPrevious, bool waitForInput, AudioClip voiceOverClip, Action onComplete)
		{
			GetWriter().Write(text, clearPrevious, waitForInput, onComplete);
		}

		protected virtual void Update()
		{
			if (continueImage != null)
			{
				continueImage.enabled = GetWriter().isWaitingForInput;
			}
		}
	}

}
