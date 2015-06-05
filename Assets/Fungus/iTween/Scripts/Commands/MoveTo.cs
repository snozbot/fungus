using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move To", 
	             "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
	[AddComponentMenu("")]
	public class MoveTo : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will move to")]
		public Transform toTransform;

		[Tooltip("Target world position that the GameObject will move to, if no From Transform is set")]
		public Vector3 toPosition;

		[Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (toTransform == null)
			{
				tweenParams.Add("position", toPosition);
			}
			else
			{
				tweenParams.Add("position", toTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveTo(targetObject, tweenParams);
		}		
	}

}