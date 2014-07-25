using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class WaitCommand : FungusCommand 
	{
		public float duration;

		public override void OnExecute()
		{
			Invoke ("OnWaitComplete", duration);
		}

		void OnWaitComplete()
		{
			ExecuteNextCommand();
		}
	}

}