using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("iTween", 
	             "Stop Tweens", 
	             "Stop all active iTweens in the current scene.")]
	public class StopTweens : Command 
	{
		public override void OnEnter()
		{
			iTween.Stop();
			Continue();
		}
	}

}