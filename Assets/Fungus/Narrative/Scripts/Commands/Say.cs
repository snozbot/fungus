/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

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
	public class Say : Command, ILocalizable
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

		[Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
		public bool fadeWhenDone = true;

		[Tooltip("Wait for player to click before continuing.")]
		public bool waitForClick = true;

		[Tooltip("Stop playing voiceover when text finishes writing.")]
		public bool stopVoiceover = true;

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
			if (character != null && character.setSayDialog != null)
			{
				SayDialog.activeSayDialog = character.setSayDialog;
			}

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

			sayDialog.gameObject.SetActive(true);

			sayDialog.SetCharacter(character, flowchart);
			sayDialog.SetCharacterImage(portrait);

			string displayText = storyText;

			foreach (CustomTag ct in CustomTag.activeCustomTags)
			{
				displayText = displayText.Replace(ct.tagStartSymbol, ct.replaceTagStartWith);
				if (ct.tagEndSymbol != "" && ct.replaceTagEndWith != "")
				{
					displayText = displayText.Replace(ct.tagEndSymbol, ct.replaceTagEndWith);
				}
			}

			string subbedText = flowchart.SubstituteVariables(displayText);

            sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, voiceOverClip, delegate {
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

		public override void OnStopExecuting()
		{
			SayDialog sayDialog = SayDialog.GetSayDialog();
			if (sayDialog == null)
			{
				return;
			}

			sayDialog.Stop();
		}

		//
		// ILocalizable implementation
		//
		
		public virtual string GetStandardText()
		{
			return storyText;
		}

		public virtual void SetStandardText(string standardText)
		{
			storyText = standardText;
		}

		public virtual string GetDescription()
		{
			return description;
		}
		
		public virtual string GetStringId()
		{
			// String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
			string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
			if (character != null)
			{
				stringId += character.nameText;
			}

			return stringId;
		}
	}

}