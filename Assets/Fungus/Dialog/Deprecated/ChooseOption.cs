using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Choose", 
	             "Presents a list of options for the player to choose from using a Choose Dialog. " + 
	             "Place Option commands after the Choose Option command to specify the player options, and terminate with an End command.")]
	[AddComponentMenu("")]
	public class ChooseOption : Command 
	{
		[Tooltip("Story text to display to prompt player to choose an option")]
		[TextArea(5,10)]
		public string chooseText = "";
		
		[Tooltip("Speaking character to use when prompting the player to choose an option")]
		public Character character;
		
		[Tooltip("Choose Dialog object to use to display the player options")]
		public ChooseDialog chooseDialog;
		
		[Tooltip("Portrait that represents speaking character")]
		public Sprite portrait;
		
		[Tooltip("Voiceover audio to play when prompting the player to choose an option")]
		public AudioClip voiceOverClip;
		
		[Tooltip("Time limit for player to choose an option. Set to 0 for no time limit.")]
		public float timeoutDuration;
		
		public override void OnEnter()
		{
			if (chooseDialog == null)
			{
				if (chooseDialog == null)
				{
					// Try to get any ChooseDialog in the scene
					chooseDialog = GameObject.FindObjectOfType<ChooseDialog>();
				}				
			}

			// Build list of Option commands
			End endCommand = null;
			List<Option> options = new List<Option>();
			for (int i = commandIndex + 1; i < parentSequence.commandList.Count; ++i)
			{
				Command command = parentSequence.commandList[i];

				// Check for closing End command
				if (command.GetType() == typeof(End) &&
				    command.indentLevel == indentLevel)
				{
					endCommand = command as End;

					// Jump to End if no dialog is set
					if (chooseDialog == null)
					{
						Continue (endCommand.commandIndex + 1);
						return;
					}

					break;
				}

				Option option = command as Option;
				if (option != null &&
				    option.indentLevel == indentLevel &&
				    !option.IsHidden())
				{
					options.Add(command as Option);
				}
			}

			if (options.Count == 0)
			{
				Continue();
			}
			else
			{
				FungusScript fungusScript = GetFungusScript();
				
				chooseDialog.ShowDialog(true);
				chooseDialog.SetCharacter(character, fungusScript);
				chooseDialog.SetCharacterImage(portrait);
				
				List<ChooseDialog.Option> dialogOptions = new List<ChooseDialog.Option>();
				foreach (Option option in options)
				{
					ChooseDialog.Option dialogOption = new ChooseDialog.Option();
					dialogOption.text = fungusScript.SubstituteVariables(option.optionText);

					Option theOption = option; // Needed to close over the option object in the delegate
					
					dialogOption.onSelect = delegate {
						chooseDialog.ShowDialog(false);
						theOption.wasSelected = true;
						Continue(theOption.commandIndex + 1);
					};
					
					dialogOptions.Add(dialogOption);
				}
				
				options.Clear();
				
				if (voiceOverClip != null)
				{
					chooseDialog.PlayVoiceOver(voiceOverClip);
				}
				
				string subbedText = fungusScript.SubstituteVariables(chooseText);
				
				chooseDialog.Choose(subbedText, dialogOptions, timeoutDuration, delegate {
					// Timeout will execute the commands listed immediately after the Choose Option command
					chooseDialog.ShowDialog(false);
					Continue();
				});
			}
		}

		public override bool OpenBlock()
		{
			return true;
		}
		
		public override string GetSummary()
		{
			return "\"" + chooseText + "\"";
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}
	
}