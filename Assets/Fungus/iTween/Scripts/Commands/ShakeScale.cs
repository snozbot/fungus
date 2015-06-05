using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Shake Scale", 
	             "Randomly shakes a GameObject's rotation by a diminishing amount over time.")]
	[AddComponentMenu("")]
	public class ShakeScale : iTweenCommand 
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
			iTween.ShakeScale(targetObject, tweenParams);
		}		
	}
	
}