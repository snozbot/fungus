using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Stop Tween", 
	             "Stops an active iTween by name.")]
	[AddComponentMenu("")]
	public class StopTween : Command 
	{
		[Tooltip("Stop and destroy any Tweens in current scene with the supplied name")]
		public string tweenName;

		public override void OnEnter()
		{
			iTween.StopByName(tweenName);
			Continue();
		}
	}

}