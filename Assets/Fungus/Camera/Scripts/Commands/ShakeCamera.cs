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
			Vector3 v = new Vector3();
			v = amount;

			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("amount", v);
			tweenParams.Add("time", duration);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ShakePosition(Camera.main.gameObject, tweenParams);
			
			if (!waitUntilFinished)
			{
				Continue();
			}
		}
		
		protected virtual void OniTweenComplete(object param)
		{
			Command command = param as Command;
			if (command != null && command.Equals(this))
			{
				if (waitUntilFinished)
				{
					Continue();
				}
			}
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