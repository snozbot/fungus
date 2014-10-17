using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate Add", 
	             "Rotates a game object by the specified angles over time.")]
	public class RotateAdd : iTweenCommand 
	{
		public Vector3 offset;
		public Space space = Space.Self;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("amount", offset);
			tweenParams.Add("space", space);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateAdd(target, tweenParams);
		}
	}

}