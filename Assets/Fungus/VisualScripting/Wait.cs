using UnityEngine;
using System;
using System.Collections;

namespace Fungus.Script
{

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