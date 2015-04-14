using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Deprecated", 
	             "Choose", 
	             "Obsolete: This command will be removed in a future update. Please use the Menu command instead.",
	             -1)]
	[AddComponentMenu("")]
	public class Choose : Command 
	{
		public class Option
		{
			public string optionText;
			[FormerlySerializedAs("targetSequence")]
			public Block targetBlock;
			public Action action;
		}
		
		static public List<Option> options = new List<Option>();
		
		[Tooltip("Story text to display to prompt player to choose an option")]
		[TextArea(5,10)]
		public string chooseText;
		
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
		
		protected bool showBasicGUI;
		
		public override void OnEnter()
		{
			showBasicGUI = false;
			if (chooseDialog == null)
			{
				if (chooseDialog == null)
				{
					// Try to get any ChooseDialog in the scene
					chooseDialog = GameObject.FindObjectOfType<ChooseDialog>();
				}
				
				if (chooseDialog == null)
				{
					showBasicGUI = true;
					return;
				}
			}
			
			if (options.Count == 0)
			{
				Continue();
			}
			else
			{
				Flowchart flowchart = GetFlowchart();
				
				chooseDialog.ShowDialog(true);
				chooseDialog.SetCharacter(character, flowchart);
				chooseDialog.SetCharacterImage(portrait);
				
				List<ChooseDialog.Option> dialogOptions = new List<ChooseDialog.Option>();
				foreach (Option option in options)
				{
					ChooseDialog.Option dialogOption = new ChooseDialog.Option();
					
					// Store these in local variables so they get closed over correctly by the delegate call
					dialogOption.text = option.optionText;
					dialogOption.text = flowchart.SubstituteVariables(dialogOption.text);
					Block onSelectBlock = option.targetBlock;
					Action optionAction = option.action;
					
					dialogOption.onSelect = delegate {
						
						if (optionAction != null)
						{
							optionAction();
						}
						
						chooseDialog.ShowDialog(false);
						
						if (onSelectBlock == null)
						{
							Continue ();
						}
						else
						{
							Stop();
							onSelectBlock.Execute();
						}
					};
					
					dialogOptions.Add(dialogOption);
				}
				
				options.Clear();
				
				if (voiceOverClip != null)
				{
					chooseDialog.PlayVoiceOver(voiceOverClip);
				}
				
				string subbedText = flowchart.SubstituteVariables(chooseText);
				
				chooseDialog.Choose(subbedText, dialogOptions, timeoutDuration, delegate {
					chooseDialog.ShowDialog(false);
					Continue();
				});
			}
		}
		
		public override string GetSummary()
		{
			return "\"" + chooseText + "\"";
		}
		
		public override void GetConnectedBlocks (ref List<Block> connectedBlocks)
		{
			// Show connected blocks from preceding AddOption commands
			if (isExecuting)
			{
				foreach (Option option in options)
				{
					if (option.targetBlock != null)
					{
						connectedBlocks.Add(option.targetBlock);
					}
				}
			}
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
			
			GUILayout.Label(chooseText);
			
			foreach (Option option in options)
			{
				if (GUILayout.Button(option.optionText))
				{
					options.Clear();
					showBasicGUI = false;
					Stop();
					if (option.targetBlock != null)
					{
						option.targetBlock.Execute();
					}
				}
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