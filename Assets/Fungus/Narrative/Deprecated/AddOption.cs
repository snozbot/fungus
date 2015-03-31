using UnityEngine;
using UnityEngine.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Deprecated", 
	             "Add Option", 
	             "Obsolete: This command will be removed in a future update. Please use the Option command instead.",
	             -1)]
	[AddComponentMenu("")]
	public class AddOption : SetVariable 
	{
		[Tooltip("Option text to display when presenting the option to the player")]
		public string optionText;

		[FormerlySerializedAs("targetSequence")]
		[Tooltip("Block to execute when the player selects this option")]
		public Block targetBlock;

		[Tooltip("Hide this option once it has been selected so that it won't appear again even if executed again")]
		public bool hideOnSelected;

		protected bool wasSelected;

		public override void OnEnter()
		{
			if (hideOnSelected && wasSelected)
			{
				Continue();
				return;
			}

			Choose.Option option = new Choose.Option();
			option.optionText = optionText; // Note: Variable substitution happens in the Choose command (as late as possible)
			option.targetBlock = targetBlock;

			option.action = () => {
				wasSelected = true;
				DoSetOperation(); // Set variable (if one is specified)
			};

			Choose.options.Add(option);

			Continue();
		}

		public override string GetSummary()
		{
			string summaryText = optionText;

			if (targetBlock == null)
			{
				summaryText += " ( <Continue> )";
			}
			else
			{
				summaryText += " (" + targetBlock.blockName + ")";
			}

			return summaryText;
		}

		public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
		{
			if (targetBlock != null)
			{
				connectedBlocks.Add(targetBlock);
			}
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}

		public override void OnReset()
		{
			wasSelected = false;
		}
	}

}