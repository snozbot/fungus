using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	[CommandCategory("Dialog")]
	[HelpText("Writes a line of story text to the dialog. A condition can be specified for when the story text should be shown.")]
	public class Say : FungusCommand 
	{
		public DialogController dialogController;
		public Character character;
		public string storyText;
		public bool displayOnce;

		[Serializable]
		public class SayOption
		{
			public string optionText;
			public Sequence targetSequence;
		}

		public List<SayOption> options = new List<SayOption>();

		public float timeoutDuration;

		int executionCount;

		public override void OnEnter()
		{
			if (dialogController == null)
			{
				Continue();
				return;
			}
			
			if (displayOnce && executionCount > 0)
			{
				Continue();
				return;
			}

			executionCount++;

			if (character != null)
			{
				dialogController.SetCharacter(character);
			}

			if (options.Count > 0)
			{
				List<DialogController.Option> dialogOptions = new List<DialogController.Option>();
				foreach (SayOption sayOption in options)
				{
					DialogController.Option dialogOption = new DialogController.Option();
					dialogOption.text = sayOption.optionText;
					Sequence onSelectSequence = sayOption.targetSequence;

					dialogOption.onSelect = delegate {
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

				dialogController.Say(storyText, dialogOptions);
			}
			else
			{
				dialogController.Say(storyText, delegate {
					Continue();
				});
			}
		}

		public override string GetSummary()
		{
			return "\"" + storyText + "\"";
		}

		public override void GetConnectedSequences (ref List<Sequence> connectedSequences)
		{
			foreach (SayOption option in options)
			{
				connectedSequences.Add(option.targetSequence);
			}
		}
	}

}