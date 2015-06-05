using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Punch Rotation", 
	             "Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation.")]
	[AddComponentMenu("")]
	public class PunchRotation : iTweenCommand 
	{
		[Tooltip("A rotation offset in space the GameObject will animate to")]
		public Vector3 amount;

		[Tooltip("Apply the transformation in either the world coordinate or local cordinate system")]
		public Space space = Space.Self;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			tweenParams.Add("amount", amount);
			tweenParams.Add("space", space);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.PunchRotation(targetObject, tweenParams);
		}
	}

}