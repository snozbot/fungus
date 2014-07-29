using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{

	public class WaitCommand : FungusCommand 
	{
		public float duration;

		public override void OnEnter()
		{
			Invoke ("OnWaitComplete", duration);
		}

		void OnWaitComplete()
		{
			Finish();
		}
	}

}