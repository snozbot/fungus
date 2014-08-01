using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public class Say : FungusCommand 
	{
		public string character;
		public string text;

		public override void OnEnter()
		{
			Dialog dialog = Game.GetInstance().dialog;
			if (dialog == null)
			{
				Continue();
				return;
			}

			if (character.Length > 0)
			{
				dialog.SetCharacter(character);
			}

			dialog.Say (text, delegate {
				Continue();
			});
		}

	}

}