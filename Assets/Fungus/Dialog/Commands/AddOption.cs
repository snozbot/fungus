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

		public override void OnEnter()
		{
			Say.SayOption option = new Say.SayOption();
			option.optionText = optionText;
			option.targetSequence = targetSequence;
			Say.options.Add(option);

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