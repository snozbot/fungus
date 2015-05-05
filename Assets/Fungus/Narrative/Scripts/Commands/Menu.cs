using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Narrative", 
	             "Menu", 
	             "Displays a button in a multiple choice menu")]
	[AddComponentMenu("")]
	public class Menu : Command
	{
		[Tooltip("Text to display on the menu button")]
		public string text = "Option Text";

		[Tooltip("Notes about the option text for other authors, localization, etc.")]
		public string description = "";

		[FormerlySerializedAs("targetSequence")]
		[Tooltip("Block to execute when this option is selected")]
		public Block targetBlock;

		[Tooltip("Hide this option if the target block has been executed previously")]
		public bool hideIfVisited;

		[Tooltip("A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.")]
		public MenuDialog setMenuDialog;

		protected static bool eventSystemPresent;

		public override void OnEnter()
		{
			CheckEventSystem();

			if (setMenuDialog != null)
			{
				// Override the active menu dialog
				MenuDialog.activeMenuDialog = setMenuDialog;
			}

			bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0);

			if (!hideOption)
			{
				MenuDialog menuDialog = MenuDialog.GetMenuDialog();
				if (menuDialog != null)
				{
					menuDialog.gameObject.SetActive(true);
					string displayText = text;
					menuDialog.AddOption(displayText, targetBlock);
				}
			}

			Continue();
		}

		// There must be an Event System in the scene for Menu input to work.
		// This function will automatically instantiate one if none exists.
		protected virtual void CheckEventSystem()
		{
			if (eventSystemPresent)
			{
				return;
			}

			EventSystem eventSystem = GameObject.FindObjectOfType<EventSystem>();
			if (eventSystem == null)
			{
				// Auto spawn an Event System from the prefab
				GameObject prefab = Resources.Load<GameObject>("EventSystem");
				if (prefab != null)
				{
					GameObject go = Instantiate(prefab) as GameObject;
					go.name = "EventSystem";
				}
			}

			eventSystemPresent = true;
		}

		public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
		{
			if (targetBlock != null)
			{
				connectedBlocks.Add(targetBlock);
			}		
		}

		public override string GetSummary()
		{
			if (targetBlock == null)
			{
				return "Error: No target block selected";
			}

			if (text == "")
			{
				return "Error: No button text selected";
			}

			return text + " : " + targetBlock.blockName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}
	}

}