using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Shake Camera", 
	             "Applies a camera shake effect to the main camera.")]
	[AddComponentMenu("")]
	public class ShakeCamera : Command 
	{
		[Tooltip("Time for camera shake effect to complete")]
		public float duration = 0.5f;

		[Tooltip("Magnitude of shake effect in x & y axes")]
		public Vector2 amount = new Vector2(1, 1);

		[Tooltip("Wait until the shake effect has finished before executing next command")]
		public bool waitUntilFinished;

		public override void OnEnter()
		{
			iTween.ShakePosition(Camera.main.gameObject, new Vector3(amount.x, amount.y, 0), duration);

			if (waitUntilFinished)
			{
				Invoke("OnWaitComplete", duration);
			}
			else
			{
				Continue();
			}
		}

		protected virtual void OnWaitComplete()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return "For " + duration + " seconds.";
		}

		public override Color GetButtonColor()
		{
			return new Color32(216, 228, 170, 255);
		}
	}

}