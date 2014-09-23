using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Add Option", 
	             "Adds an option for the player to select, displayed by the next Say command.")]
	public class AddOption : Command 
	{
		public string optionText;
		public Sequence targetSequence;

		public bool hideOnSelected;

		public override void OnEnter()
		{
			if (hideOnSelected &&
			    targetSequence.GetExecutionCount() > 0)
			{
				Continue();
				return;
			}

			Choose.Option option = new Choose.Option();
			option.optionText = optionText;
			option.targetSequence = targetSequence;
			Choose.options.Add(option);

			Continue();
		}

		public override string GetSummary()
		{
			string summaryText = optionText;

			if (targetSequence == null)
			{
				summaryText += " ( <Continue> )";
			}
			else
			{
				summaryText += " (" + targetSequence.name + ")";
			}

			return summaryText;
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
			}
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}