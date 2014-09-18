using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Camera", 
	             "Shake Camera", 
	             "Applies a camera shake effect to the main camera.")]
	public class ShakeCamera : Command 
	{
		public float duration = 0.5f;
		public Vector2 amount = new Vector2(1, 1);
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