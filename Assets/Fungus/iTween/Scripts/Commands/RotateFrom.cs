using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate From", 
	             "Rotates a game object from the specified angles back to its starting orientation over time.")]
	[AddComponentMenu("")]
	public class RotateFrom : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will rotate from")]
		public Transform fromTransform;

		[Tooltip("Target rotation that the GameObject will rotate from, if no From Transform is set")]
		public Vector3 fromRotation;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (fromTransform == null)
			{
				tweenParams.Add("rotation", fromRotation);
			}
			else
			{
				tweenParams.Add("rotation", fromTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateFrom(targetObject, tweenParams);
		}		
	}

}