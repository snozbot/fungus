using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Say", 
	             "Writes a line of story text to the dialog. A list of options can be specified for the player to choose from. Use a non-zero timeout to give the player a limited time to choose.")]
	public class Say : Command 
	{
		[TextArea(5,10)]
		public string storyText;

		public Character character;
		public AudioClip voiceOverClip;
		public bool showOnce;
		protected int executionCount;
		protected bool showBasicGUI;

		public override void OnEnter()
		{
			if (showOnce && executionCount > 0)
			{
				Continue();
				return;
			}
			
			executionCount++;

			SayDialog dialog = SetSayDialog.activeDialog;
			showBasicGUI = false;
			if (dialog == null)
			{
				// Try to get any SayDialog in the scene
				dialog = GameObject.FindObjectOfType<SayDialog>();
				if (dialog == null)
				{
					showBasicGUI = true;
					return;
				}
			}
	
			dialog.SetCharacter(character);

			dialog.ShowDialog(true);

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
			return "\"" + storyText + "\"";
		}

		protected virtual void OnGUI()
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
				GUILayout.Label(character.nameText);
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

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}