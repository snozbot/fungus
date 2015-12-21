using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Fade To View", 
	             "Fades the camera out and in again at a position specified by a View object.")]
	[AddComponentMenu("")]
	public class FadeToView : Command 
	{
		[Tooltip("Time for fade effect to complete")]
		public float duration = 1f;

		[Tooltip("Fade from fully visible to opaque at start of fade")]
		public bool fadeOut = true;

		[Tooltip("View to transition to when Fade is complete")]
		public View targetView;

		[Tooltip("Wait until the fade has finished before executing next command")]
		public bool waitUntilFinished = true;

		[Tooltip("Color to render fullscreen fade texture with when screen is obscured.")]
		public Color fadeColor = Color.black;

		[Tooltip("Optional texture to use when rendering the fullscreen fade effect.")]
		public Texture2D fadeTexture;

		[Tooltip("Camera to use for the fade. Will use main camera if set to none.")]
		public Camera targetCamera;

		public virtual void Start()
		{
			if (targetCamera == null)
			{
				targetCamera = Camera.main;
			}
			if (targetCamera == null)
			{
				targetCamera = GameObject.FindObjectOfType<Camera>();
			}
		}

		public override void OnEnter()
		{
			if (targetCamera == null ||
			    targetView == null)
			{
				Continue();
				return;
			}

			CameraController cameraController = CameraController.GetInstance();

			if (waitUntilFinished)
			{
				cameraController.waiting = true;
			}

			if (fadeTexture)
			{
				cameraController.screenFadeTexture = fadeTexture;
			}
			else
			{
				cameraController.screenFadeTexture = CameraController.CreateColorTexture(fadeColor, 32, 32);
			}

			cameraController.FadeToView(targetCamera, targetView, duration, fadeOut, delegate {	
				if (waitUntilFinished)
				{
					cameraController.waiting = false;
					Continue();
				}
			});

			if (!waitUntilFinished)
			{
				Continue();
			}
		}

		public override void OnStopExecuting()
		{
			CameraController.GetInstance().StopAllCoroutines();
		}

		public override string GetSummary()
		{
			if (targetView == null)
			{
				return "Error: No view selected";
			}
			else
			{
				return targetView.name;
			}
		}

		public override Color GetButtonColor()
		{
			return new Color32(216, 228, 170, 255);
		}
	}

}