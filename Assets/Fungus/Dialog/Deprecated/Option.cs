using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Option", 
	             "Adds an option for the player to select. When the option is selected all commands in the following block are executed.")]
	[AddComponentMenu("")]
	public class Option : If 
	{
		[Tooltip("Option text to display when presenting the option to the player")]
		public string optionText = "";

		[Tooltip("Hide this option once it has been selected so that it won't appear again even if executed again")]
		public bool hideOnSelected;

		[Tooltip("Hide if a variable condition evaluates to true.")]
		public bool hideOnCondition;

		[NonSerialized]
		public bool wasSelected;

		public virtual bool IsHidden()
		{
			if (hideOnSelected && wasSelected)
			{
				return true;
			}

			if (hideOnCondition)
			{
				// If no variable is selected then assume the option is visible
				if (variable == null)
				{
					return true;
				}

				return EvaluateCondition();
			}

			return false;
		}

		public override void OnEnter()
		{
			// Find next End statement at same indent level
			for (int i = commandIndex + 1; i < parentSequence.commandList.Count; ++i)
			{
				End endCommand = parentSequence.commandList[i] as End;

				if (endCommand != null && 
				    endCommand.indentLevel == indentLevel)
				{
					// Continue at next command after End
					Continue (endCommand.commandIndex + 1);
					return;
				}
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (optionText == "")
			{
				return "Error: Option text is blank";
			}

			return optionText;
		}

		public override bool OpenBlock()
		{
			return true;
		}


		public override bool CloseBlock ()
		{
			return true;
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}

		public override void OnReset()
		{
			wasSelected = false;
		}
	}

}