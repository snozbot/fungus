using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate From", 
	             "Rotates a game object from the specified angles back to its starting orientation over time.")]
	public class RotateFrom : iTweenCommand 
	{
		public Transform fromTransform;
		public Vector3 fromRotation;
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			if (fromTransform == null)
			{
				tweenParams.Add("rotation", fromRotation);
			}
			else
			{
				tweenParams.Add("rotation", fromTransform);
			}
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateFrom(target, tweenParams);
		}		
	}

}