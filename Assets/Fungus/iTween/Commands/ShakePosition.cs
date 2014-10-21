using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Shake Position", 
	             "Randomly shakes a GameObject's position by a diminishing amount over time.")]
	public class ShakePosition : iTweenCommand 
	{
		public Vector3 amount;
		public bool isLocal;
		public iTweenAxis axis;
		
		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			tweenParams.Add("amount", amount);
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
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ShakePosition(target, tweenParams);
		}		
	}
	
}