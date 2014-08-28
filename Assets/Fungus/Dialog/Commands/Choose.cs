using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandCategory("Dialog")]
	[HelpText("Presents a list of options for the player to choose from, with an optional timeout. Add options using preceding AddOption commands.")]
	public class Choose : FungusCommand 
	{
		public ChooseDialog dialog;
		static public ChooseDialog activeDialog;

		public class Option
		{
			public string optionText;
			public Sequence targetSequence;
		}

		static public List<Option> options = new List<Option>();

		public string chooseText;
		public Character character;
		public float timeoutDuration;

		public override void OnEnter()
		{
			// Remember active dialog between Choose calls
			if (dialog == null)
			{
				if (Choose.activeDialog == null)
				{
					Continue();
					return;
				}
				else
				{
					dialog = Choose.activeDialog;
				}
			}
			else
			{
				activeDialog = dialog;
			}

			dialog.ShowDialog(true);

			dialog.SetCharacter(character);

			if (options.Count == 0)
			{
				Continue();
			}
			else
			{
				List<ChooseDialog.Option> dialogOptions = new List<ChooseDialog.Option>();
				foreach (Option option in options)
				{
					ChooseDialog.Option dialogOption = new ChooseDialog.Option();
					dialogOption.text = option.optionText;
					Sequence onSelectSequence = option.targetSequence;

					dialogOption.onSelect = delegate {

						dialog.ShowDialog(false);

						if (onSelectSequence == null)
						{
							Continue ();
						}
						else
						{
							ExecuteSequence(onSelectSequence);
						}
					};

					dialogOptions.Add(dialogOption);
				}
			
				options.Clear();

				dialog.Choose(chooseText, dialogOptions, timeoutDuration, delegate {
					dialog.ShowDialog(false);
					Continue();
				});
			}
		}

		public override string GetSummary()
		{
			string summary = "";
			if (character != null)
			{
				summary = character.characterName + ": ";
			}

			summary += "\"" + chooseText + "\"";

			return summary;
		}

		public override void GetConnectedSequences (ref List<Sequence> connectedSequences)
		{
			foreach (Option option in options)
			{
				if (option.targetSequence != null)
				{
					connectedSequences.Add(option.targetSequence);
				}
			}
		}
	}

}