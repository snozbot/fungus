using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandCategory("Camera")]
	[CommandName("Fade To View")]
	[HelpText("Fades the camera out and in again at a location specified by a View object.")]
	public class FadeToView : FungusCommand 
	{
		public float duration;
		public Fungus.View targetView;
		public bool waitUntilFinished = true;
		public Color fadeColor = Color.black;
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

			cameraController.FadeToView(targetView, duration, delegate {	
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
	}

}