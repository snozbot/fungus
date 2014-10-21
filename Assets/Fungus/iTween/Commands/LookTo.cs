using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Look To", 
	             "Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
	public class LookTo : iTweenCommand 
	{
		public Transform toTransform;
		public Vector3 toPosition;
		public iTweenAxis axis;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			if (toTransform == null)
			{
				tweenParams.Add("looktarget", toPosition);
			}
			else
			{
				tweenParams.Add("looktarget", toTransform);
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
			iTween.LookTo(target, tweenParams);
		}		
	}

}