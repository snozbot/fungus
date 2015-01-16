using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Stop Swipe", 
	             "Deactivates swipe panning mode.")]
	[AddComponentMenu("")]
	public class StopSwipe : Command 
	{
		public override void OnEnter()
		{
			CameraController cameraController = CameraController.GetInstance();

			cameraController.StopSwipePan();

			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(216, 228, 170, 255);
		}
	}

}