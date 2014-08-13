using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	public class Say : FungusCommand 
	{
		public enum ShowCondition
		{
			Always,
			Once,
			BooleanIsTrue,
			BooleanIsFalse
		}
		
		public string character;
		public string text;
		public ShowCondition showCondition;
		public BooleanVariable booleanVariable;

		int executionCount;

		public override void OnEnter()
		{
			Dialog dialog = Game.GetInstance().dialog;
			if (dialog == null)
			{
				Continue();
				return;
			}
			
			bool showSayText = true;
			
			if (showCondition == ShowCondition.Always) 
			{
				// Always show option
			}
			else if (showCondition == ShowCondition.Once) 
			{
				if (executionCount > 0)
				{
					showSayText = false;	
				}
			}
			else
			{
				if (booleanVariable == null)
				{
					showSayText = false;
				}
				else
				{
					if (showCondition == ShowCondition.BooleanIsTrue &&
					    booleanVariable.Value != true)
					{
						showSayText = false;
					}
					else if (showCondition == ShowCondition.BooleanIsFalse &&
					         booleanVariable.Value != false)
					{
						showSayText = false;
					}
				}				
			}
			
			if (!showSayText)
			{
				Continue();
				return;
			}

			executionCount++;

			if (character.Length > 0)
			{
				dialog.SetCharacter(character);
			}

			dialog.Say (text, delegate {
				Continue();
			});
		}

		public override string GetDescription()
		{
			return "\"" + text + "\"";
		}
	}

}