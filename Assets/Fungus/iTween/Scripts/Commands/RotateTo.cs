using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate To", 
	             "Rotates a game object to the specified angles over time.")]
	[AddComponentMenu("")]
	public class RotateTo : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will rotate to")]
		public Transform toTransform;

		[Tooltip("Target rotation that the GameObject will rotate to, if no To Transform is set")]
		public Vector3 toRotation;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (toTransform == null)
			{
				tweenParams.Add("rotation", toRotation);
			}
			else
			{
				tweenParams.Add("rotation", toTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateTo(targetObject, tweenParams);
		}		
	}

}