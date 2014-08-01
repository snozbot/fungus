using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public class SayCommand : FungusCommand 
	{
		public string text;

		public override void OnEnter()
		{
			Dialog dialog = Game.GetInstance().dialog;
			if (dialog == null)
			{
				Continue();
				return;
			}

			dialog.Say (text, delegate {
				Continue();
			});
		}

	}

}