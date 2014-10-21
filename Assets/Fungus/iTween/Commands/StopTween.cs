using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Stop Tween", 
	             "Stops an active iTween by name.")]
	public class StopTween : Command 
	{
		public string tweenName;

		public override void OnEnter()
		{
			iTween.StopByName(tweenName);
			Continue();
		}
	}

}