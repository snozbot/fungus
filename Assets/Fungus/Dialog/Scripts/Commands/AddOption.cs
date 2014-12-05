using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Add Option", 
	             "Adds an option for the player to select. All previously added options are displayed by the next Choose command. " + 
	             "You can specify another sequence to call and/or a variable to set when the options is selected.")]
	public class AddOption : SetVariable 
	{
		[Tooltip("Option text to display when presenting the option to the player")]
		public string optionText;

		[Tooltip("Sequence to execute when the player selects this option")]
		public Sequence targetSequence;

		[Tooltip("Hide this option once it has been selected so that it won't appear again even if executed again")]
		public bool hideOnSelected;

		protected bool wasSelected;

		public override void OnEnter()
		{
			if (hideOnSelected && wasSelected)
			{
				Continue();
				return;
			}

			Choose.Option option = new Choose.Option();
			option.optionText = optionText; // Note: Variable substitution happens in the Choose command (as late as possible)
			option.targetSequence = targetSequence;

			option.action = () => {
				wasSelected = true;
				DoSetOperation(); // Set variable (if one is specified)
			};

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
				summaryText += " (" + targetSequence.sequenceName + ")";
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

		public override void OnReset()
		{
			wasSelected = false;
		}
	}

}