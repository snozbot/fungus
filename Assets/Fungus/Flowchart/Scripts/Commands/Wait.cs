using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Wait", 
	             "Waits for period of time before executing the next command in the block.")]

	[AddComponentMenu("")]
	public class Wait : Command 
	{
		[Tooltip("Duration to wait for")]
		public float duration = 1;

		public override void OnEnter()
		{
			Invoke ("OnWaitComplete", duration);
		}

		void OnWaitComplete()
		{
			Continue();
		}

		public override string GetSummary()
		{
			return duration.ToString() + " seconds";
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}