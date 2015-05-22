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

		public override void OnEnter()
		{
			if (targetView == null)
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

			cameraController.FadeToView(targetView, duration, fadeOut, delegate {	
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