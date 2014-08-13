using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{

	public class View : FungusCommand 
	{
		public enum Transition
		{
			Move,
			Fade
		}

		public Transition transition;
		public float duration;
		public Fungus.View targetView;
		public bool waitUntilFinished = true;

		public override void OnEnter()
		{
			Game game = Game.GetInstance();

			if (waitUntilFinished)
			{
				game.waiting = true;
			}

			if (transition == Transition.Fade)
			{
				game.cameraController.FadeToView(targetView, duration, delegate {	
					if (waitUntilFinished)
					{
						game.waiting = false;
						Continue();
					}
				});
			}
			else if (transition == Transition.Move)
			{
				Vector3 targetPosition = targetView.transform.position;
				Quaternion targetRotation = targetView.transform.rotation;
				float targetSize = targetView.viewSize;

				game.cameraController.PanToPosition(targetPosition, targetRotation, targetSize, duration, delegate {
					if (waitUntilFinished)
					{
						game.waiting = false;
						Continue();
					}
				});
			}

			if (!waitUntilFinished)
			{
				Continue();
			}
		}
	}

}