using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale To", 
	             "Changes a game object's scale to a specified value over time.")]
	public class ScaleTo : iTweenCommand 
	{
		public Vector3 scale;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("scale", scale);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleTo(target, tweenParams);
		}		
	}

}