using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandInfo("Camera", 
	             "Move To View", 
	             "Moves the camera to a location specified by a View object.", 
	             216, 228, 170)]
	public class MoveToView : FungusCommand 
	{
		public float duration;
		public Fungus.View targetView;
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			CameraController cameraController = CameraController.GetInstance();

			if (waitUntilFinished)
			{
				cameraController.waiting = true;
			}

			Vector3 targetPosition = targetView.transform.position;
			Quaternion targetRotation = targetView.transform.rotation;
			float targetSize = targetView.viewSize;

			cameraController.PanToPosition(targetPosition, targetRotation, targetSize, duration, delegate {
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