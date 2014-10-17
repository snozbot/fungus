using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move To", 
	             "Moves a game object to a specified position over time.")]
	public class MoveTo : iTweenCommand 
	{
		public Vector3 position;
		public bool isLocal;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("position", position);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("isLocal", isLocal);
			tweenParams.Add("oncomplete", "OnComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveTo(target, tweenParams);
		}		
	}

}