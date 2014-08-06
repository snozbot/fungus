using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	public class AddOption : FungusCommand
	{
		public enum ShowCondition
		{
			Always,
			NotVisited,
			BooleanIsTrue,
			BooleanIsFalse
		}
		
		public string optionText;
		public Sequence targetSequence;
		public ShowCondition showCondition;
		public string booleanVariableKey;

		public override void OnEnter()
		{
			Dialog dialog = Game.GetInstance().dialog;

			bool showOption = (dialog != null && targetSequence != null);

			if (showCondition == ShowCondition.Always) 
			{
				// Always show option
			}
			else if (showCondition == ShowCondition.NotVisited) 
			{
				if (targetSequence.GetExecutionCount () > 0)
				{
					showOption = false;	
				}
			}
			else
			{
				Variable v = parentFungusScript.GetVariable(booleanVariableKey);
				if (v == null)
				{
					showOption = false;
				}
				else
				{
					if (showCondition == ShowCondition.BooleanIsTrue &&
						v.booleanValue != true)
					{
						showOption = false;
					}
					else if (showCondition == ShowCondition.BooleanIsFalse &&
					    	 v.booleanValue != false)
			    	{
						showOption = false;
					}
				}				
			}

			if (showOption)
			{
				dialog.AddOption(optionText, () => {
					Stop();
					parentFungusScript.ExecuteSequence(targetSequence); 
				});
			}
			Continue();
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
			}
		}

	}
	
}