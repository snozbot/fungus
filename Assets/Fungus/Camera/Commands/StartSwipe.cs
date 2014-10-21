using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Start Swipe", 
	             "Activates swipe panning mode where the player can pan the camera within the area between viewA & viewB.")]
	public class StartSwipe : Command 
	{
		public View viewA;
		public View viewB;
		public float duration = 0.5f;
		
		public override void OnEnter()
		{
			CameraController cameraController = CameraController.GetInstance();

			cameraController.StartSwipePan(viewA, viewB, duration, () => Continue() );
		}

		public override string GetSummary()
		{
			if (viewA == null)
			{
				return "Error: No view selected for View A";
			}

			if (viewB == null)
			{
				return "Error: No view selected for View B";
			}

			return viewA.name + " to " + viewB.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(216, 228, 170, 255);
		}
	}

}