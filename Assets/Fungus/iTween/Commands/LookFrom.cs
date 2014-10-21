using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Look From", 
	             "Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time.")]
	public class LookFrom : iTweenCommand 
	{
		public Transform fromTransform;
		public Vector3 fromPosition;
		public iTweenAxis axis;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (fromTransform == null)
			{
				tweenParams.Add("looktarget", fromPosition);
			}
			else
			{
				tweenParams.Add("looktarget", fromTransform);
			}
			switch (axis)
			{
			case iTweenAxis.X:
				tweenParams.Add("axis", "x");
				break;
			case iTweenAxis.Y:
				tweenParams.Add("axis", "y");
				break;
			case iTweenAxis.Z:
				tweenParams.Add("axis", "z");
				break;
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.LookFrom(target, tweenParams);
		}		
	}

}