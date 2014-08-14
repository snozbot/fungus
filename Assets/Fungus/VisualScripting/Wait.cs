using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{
	[HelpText("Waits for period of time before executing the next command in the sequence.")]
	public class Wait : FungusCommand 
	{
		public float duration;

		public override void OnEnter()
		{
			Invoke ("OnWaitComplete", duration);
		}

		void OnWaitComplete()
		{
			Continue();
		}
	}

}