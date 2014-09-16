using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Fade Screen", 
	             "Draws a fullscreen texture over the scene to give a fade effect. Target alpha 1 will obscure the screen, alpha 0 will reveal the screen.")]
	public class FadeScreen : Command 
	{
		public float duration = 1f;
		public float targetAlpha = 1f;
		public bool waitUntilFinished = true;
		public Color fadeColor = Color.black;
		public Texture2D fadeTexture;
		
		public override void OnEnter()
		{
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
			
			cameraController.Fade(targetAlpha, duration, delegate {	
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
			return "Fade to " + targetAlpha + " over " + duration + " seconds";
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(216, 228, 170, 255);
		}
	}
	
}