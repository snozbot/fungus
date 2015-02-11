using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Say", 
	             "Writes a line of story text to a Say Dialog. " +
	             "Select [Game Object > Fungus > Dialog > Say Dialog] to create a new Say Dialog in your scene. " + 
	             "Select [Game Object > Fungus > Dialog > Character] to create a new selectable speaking character.")]
	[AddComponentMenu("")]
	public class Say : Command 
	{
		[Tooltip("Story text to display to the player")]
		[TextArea(5,10)]
		public string storyText;

		[Tooltip("Speaking character to use when writing the story text")]
		public Character character;

		[Tooltip("Portrait that represents speaking character")]
		public Sprite portrait;

		[Tooltip("Voiceover audio to play when writing the story text")]
		public AudioClip voiceOverClip;

		[Tooltip("Always show this Say text when the command is executed multiple times")]
		public bool showAlways = true;

		[Tooltip("Number of times to show this Say text when the command is executed multiple times")]
		public int showCount = 1;

		[Tooltip("Wait for player input before hiding the dialog and continuing. If false then the dialog will display and execution will continue.")]
		public bool waitForInput = true;

		protected int executionCount;

		public override void OnEnter()
		{
			if (!showAlways && executionCount >= showCount)
			{
				Continue();
				return;
			}

			executionCount++;

			SayDialog sayDialog = SetSayDialog.GetActiveSayDialog();

			if (sayDialog == null)
			{
				Continue();
				return;
			}
	
			FungusScript fungusScript = GetFungusScript();
			sayDialog.SetCharacter(character, fungusScript);
			sayDialog.SetCharacterImage(portrait);

			sayDialog.ShowDialog(true);

			if (voiceOverClip != null)
			{
				sayDialog.PlayVoiceOver(voiceOverClip);
			}

			string subbedText = fungusScript.SubstituteVariables(storyText);

			sayDialog.Say(subbedText, waitForInput, delegate {
				if (waitForInput)
				{
					sayDialog.ShowDialog(false);
				}
				Continue();
			});
		}

		public override string GetSummary()
		{
			string namePrefix = "";
			if (character != null) 
			{
				namePrefix = character.nameText + ": ";
			}
			return namePrefix + "\"" + storyText + "\"";
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}

		public override void OnReset()
		{
			executionCount = 0;
		}
	}

}