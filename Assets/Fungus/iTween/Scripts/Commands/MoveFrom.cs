using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move From", 
	             "Moves a game object from a specified position back to its starting position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
	[AddComponentMenu("")]
	public class MoveFrom : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will move from")]
		public Transform fromTransform;

		[Tooltip("Target world position that the GameObject will move from, if no From Transform is set")]
		public Vector3 fromPosition;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (fromTransform == null)
			{
				tweenParams.Add("position", fromPosition);
			}
			else
			{
				tweenParams.Add("position", fromTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveFrom(targetObject, tweenParams);
		}		
	}

}