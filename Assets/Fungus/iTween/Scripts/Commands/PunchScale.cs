using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Punch Scale", 
	             "Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale.")]
	[AddComponentMenu("")]
	public class PunchScale : iTweenCommand 
	{
		[Tooltip("A scale offset in space the GameObject will animate to")]
		public Vector3 amount;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			tweenParams.Add("amount", amount);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.PunchScale(targetObject, tweenParams);
		}
	}

}