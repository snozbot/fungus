using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandName("Add Option")]
	[CommandCategory("Dialog")]
	[HelpText("Adds an option for the player to select, displayed by the next Say command.")]
	public class AddOption : FungusCommand 
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
				connectedSequences.Add (targetSequence);
			}
		}
	}

}