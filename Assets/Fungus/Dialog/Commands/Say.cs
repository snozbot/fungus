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
		static public Dialog dialogController;

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

			dialogController.ShowDialog(true);

			dialogController.SetCharacter(character);

			if (options.Count > 0)
			{
				List<Dialog.Option> dialogOptions = new List<Dialog.Option>();
				foreach (SayOption sayOption in options)
				{
					Dialog.Option dialogOption = new Dialog.Option();
					dialogOption.text = sayOption.optionText;
					Sequence onSelectSequence = sayOption.targetSequence;

					dialogOption.onSelect = delegate {

						dialogController.ShowDialog(false);

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
					dialogController.ShowDialog(false);
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