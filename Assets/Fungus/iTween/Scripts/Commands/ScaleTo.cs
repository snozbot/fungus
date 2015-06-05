using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale To", 
	             "Changes a game object's scale to a specified value over time.")]
	[AddComponentMenu("")]
	public class ScaleTo : iTweenCommand 
	{
		[Tooltip("Target transform that the GameObject will scale to")]
		public Transform toTransform;

		[Tooltip("Target scale that the GameObject will scale to, if no To Transform is set")]
		public Vector3 toScale = new Vector3(1f, 1f, 1f);

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (toTransform == null)
			{
				tweenParams.Add("scale", toScale);
			}
			else
			{
				tweenParams.Add("scale", toTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleTo(targetObject, tweenParams);
		}		
	}

}