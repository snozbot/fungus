using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandInfo("Dialog", 
	             "Choose", 
	             "Presents a list of options for the player to choose from, with an optional timeout. Add options using preceding AddOption commands.", 
	             1,1,1)]
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
		public AudioClip voiceOverClip;
		public float timeoutDuration;

		bool showBasicGUI;

		public override void OnEnter()
		{
			// Remember active dialog between Choose calls
			if (dialog == null)
			{
				if (Choose.activeDialog == null)
				{
					showBasicGUI = true;
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

			if (options.Count == 0)
			{
				Continue();
			}
			else
			{
				dialog.ShowDialog(true);
				dialog.SetCharacter(character);

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

				if (voiceOverClip != null)
				{
					MusicController.GetInstance().PlaySound(voiceOverClip, 1f);
				}

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

			GUILayout.Label(chooseText);

			foreach (Option option in options)
			{
				if (GUILayout.Button(option.optionText))
				{
					options.Clear();
					showBasicGUI = false;
					ExecuteSequence(option.targetSequence);
				}
			}

			GUILayout.EndVertical();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
	}

}