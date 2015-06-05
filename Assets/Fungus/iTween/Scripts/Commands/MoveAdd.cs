using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Move Add", 
	             "Moves a game object by a specified offset over time.")]
	[AddComponentMenu("")]
	public class MoveAdd : iTweenCommand 
	{
		[Tooltip("A translation offset in space the GameObject will animate to")]
		public Vector3 offset;

		[Tooltip("Apply the transformation in either the world coordinate or local cordinate system")]
		public Space space = Space.Self;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			tweenParams.Add("amount", offset);
			tweenParams.Add("space", space);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.MoveAdd(targetObject, tweenParams);
		}
	}

}