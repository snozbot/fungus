using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move To", 
	             "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
	public class MoveTo : iTweenCommand 
	{
		public Transform toTransform;
		public Vector3 toPosition;
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
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
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveTo(target, tweenParams);
		}		
	}

}