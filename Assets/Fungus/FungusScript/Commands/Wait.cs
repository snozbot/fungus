using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[CommandInfo("Scripting", 
	             "Wait", 
	             "Waits for period of time before executing the next command in the sequence.", 
	             253, 253, 150)]
	public class Wait : FungusCommand 
	{
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
	}

}