using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale From", 
	             "Changes a game object's scale to the specified value and back to its original scale over time.")]
	[AddComponentMenu("")]
	public class ScaleFrom : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will scale from")]
		public Transform fromTransform;

		[Tooltip("Target scale that the GameObject will scale from, if no From Transform is set")]
		public Vector3 fromScale;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (fromTransform == null)
			{
				tweenParams.Add("scale", fromScale);
			}
			else
			{
				tweenParams.Add("scale", fromTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleFrom(targetObject, tweenParams);
		}		
	}

}