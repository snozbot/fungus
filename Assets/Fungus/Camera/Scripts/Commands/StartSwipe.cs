using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Start Swipe", 
	             "Activates swipe panning mode where the player can pan the camera within the area between viewA & viewB.")]
	[AddComponentMenu("")]
	public class StartSwipe : Command 
	{
		[Tooltip("Defines one extreme of the scrollable area that the player can pan around")]
		public View viewA;

		[Tooltip("Defines one extreme of the scrollable area that the player can pan around")]
		public View viewB;

		[Tooltip("Time to move the camera to a valid starting position between the two views")]
		public float duration = 0.5f;

		[Tooltip("Multiplier factor for speed of swipe pan")]
		public float speedMultiplier = 1f;

		public override void OnEnter()
		{
			CameraController cameraController = CameraController.GetInstance();

			cameraController.StartSwipePan(viewA, viewB, duration, speedMultiplier, () => Continue() );
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