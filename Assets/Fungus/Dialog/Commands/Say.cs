using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandCategory("Dialog")]
	[HelpText("Writes a line of story text to the dialog. A list of options can be specified for the player to choose from. Use a non-zero timeout to give the player a limited time to choose.")]
	public class Say : FungusCommand 
	{
		public SayDialog dialog;
	
		static public SayDialog activeDialog;

		public Character character;
		public string storyText;
		public AudioClip voiceOverClip;
		public bool showOnce;
		int executionCount;

		bool showBasicGUI;

		public override void OnEnter()
		{
			if (showOnce && executionCount > 0)
			{
				Continue();
				return;
			}
			
			executionCount++;

			// Remember active dialog between Say calls
			if (dialog == null)
			{
				if (activeDialog == null)
				{
					showBasicGUI = true;
					return;
				}
				else
				{
					dialog = activeDialog;
				}
			}
			else
			{
				activeDialog = dialog;
			}
	
			dialog.ShowDialog(true);

			dialog.SetCharacter(character);

			if (voiceOverClip != null)
			{
				MusicController.GetInstance().PlaySound(voiceOverClip, 1f);
			}

			dialog.Say(storyText, delegate {
				dialog.ShowDialog(false);
				Continue();
			});
		}

		public override string GetSummary()
		{
			string summary = "";
			if (character != null)
			{
				summary = character.characterName + ": ";
			}

			summary += "\"" + storyText + "\"";

			return summary;
		}

		void OnGUI()
		{
			if (!showBasicGUI)
			{
				return;
			}

			// Draw a basic GUI to use when no uGUI dialog has been set
			// Does not support drawing character images

			GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical(GUILayout.Height(Screen.height));
			GUILayout.FlexibleSpace();

			GUILayout.BeginVertical(new GUIStyle(GUI.skin.box));

			if (character != null)
			{
				GUILayout.Label(character.characterName);
				GUILayout.Space(10);
			}

			GUILayout.Label(storyText);
			if (GUILayout.Button("Continue"))
			{
				showBasicGUI = false;
				Continue();
			}

			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}

}