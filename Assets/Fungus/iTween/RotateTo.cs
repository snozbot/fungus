using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Rotate To", 
	             "Rotates a game object to the specified angles over time.")]
	public class RotateTo : iTweenCommand 
	{
		public Vector3 rotation;
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("rotation", rotation);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.RotateTo(target, tweenParams);
		}		
	}

}