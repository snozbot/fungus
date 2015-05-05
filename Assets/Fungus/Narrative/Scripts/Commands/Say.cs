using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Say", 
	             "Writes text in a dialog box.")]
	[AddComponentMenu("")]
	public class Say : Command
	{
		// Removed this tooltip as users's reported it obscures the text box
		[TextArea(5,10)]
		public string storyText = "";

		[Tooltip("Notes about this story text for other authors, localization, etc.")]
		public string description = "";

		[Tooltip("Character that is speaking")]
		public Character character;

		[Tooltip("Portrait that represents speaking character")]
		public Sprite portrait;

		[Tooltip("Voiceover audio to play when writing the text")]
		public AudioClip voiceOverClip;

		[Tooltip("Always show this Say text when the command is executed multiple times")]
		public bool showAlways = true;

		[Tooltip("Number of times to show this Say text when the command is executed multiple times")]
		public int showCount = 1;

		[Tooltip("Type this text in the previous dialog box.")]
		public bool extendPrevious = false;

		[Tooltip("Fade in this dialog box.")]
		public bool fadeIn = false;

		[Tooltip("Fade out this dialog box.")]
		public bool fadeOut = false;

		[Tooltip("Wait for player to click before hiding the dialog and continuing. If false then the dialog will display and execution will continue immediately.")]
		public bool waitForClick = true;

		[Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
		public SayDialog setSayDialog;

		protected int executionCount;

		public override void OnEnter()
		{
			if (!showAlways && executionCount >= showCount)
			{
				Continue();
				return;
			}

			executionCount++;

			// Override the active say dialog if needed
			if (setSayDialog != null)
			{
				SayDialog.activeSayDialog = setSayDialog;
			}

			SayDialog sayDialog = SayDialog.GetSayDialog();

			if (sayDialog == null)
			{
				Continue();
				return;
			}
	
			Flowchart flowchart = GetFlowchart();
			sayDialog.SetCharacter(character, flowchart);
			sayDialog.SetCharacterImage(portrait);

			bool fadingIn = false;
			bool movingIn = false;
			if (sayDialog.alwaysFadeDialog || fadeIn)
			{
				sayDialog.FadeInDialog();
				fadingIn = true;
			}
			if (sayDialog.alwaysMoveDialog)
			{
				sayDialog.MoveInDialog();
				movingIn = true;
			}
			if (!fadingIn && !movingIn)
			{
				sayDialog.ShowDialog(true);
			}

			if (voiceOverClip != null)
			{
				sayDialog.PlayVoiceOver(voiceOverClip);
			}

			string displayText = storyText;

			foreach (CustomTag ct in CustomTag.activeCustomTags)
			{
				displayText = displayText.Replace(ct.tagStartSymbol,ct.replaceTagStartWith);
				if (ct.tagEndSymbol != "" && ct.replaceTagEndWith != "")
				{
					displayText = displayText.Replace(ct.tagEndSymbol,ct.replaceTagEndWith);
				}
			}

			if (extendPrevious)
			{
				displayText = "{s=0}" + Dialog.prevStoryText + "{/s}" + displayText;
			}

			string subbedText = flowchart.SubstituteVariables(displayText);

			sayDialog.Say(subbedText, waitForClick, delegate {
				if (waitForClick)
				{
					bool fadingOut = false;
					bool movingOut = false;
					if (sayDialog.alwaysFadeDialog || fadeOut)
					{
						sayDialog.FadeOutDialog();
						fadingOut = true;
					}
					if (sayDialog.alwaysMoveDialog)
					{
						sayDialog.MoveOutDialog();
						movingOut = true;
					}
					if (!fadingOut && !movingOut)
					{
						sayDialog.ShowDialog(false);
					}
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
			if (extendPrevious)
			{
				namePrefix = "EXTEND" + ": ";
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