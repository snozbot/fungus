using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Scale Add", 
	             "Changes a game object's scale by a specified offset over time.")]
	[AddComponentMenu("")]
	public class ScaleAdd : iTweenCommand 
	{
		[Tooltip("A scale offset in space the GameObject will animate to")]
		public Vector3 offset;

		public override void DoTween()
		{
			Hashtable tweenParams = new Hashtable();
			tweenParams.Add("name", tweenName);
			tweenParams.Add("amount", offset);
			tweenParams.Add("time", duration);
			tweenParams.Add("easetype", easeType);
			tweenParams.Add("looptype", loopType);
			tweenParams.Add("oncomplete", "OniTweenComplete");
			tweenParams.Add("oncompletetarget", gameObject);
			tweenParams.Add("oncompleteparams", this);
			iTween.ScaleAdd(targetObject, tweenParams);
		}
	}

}