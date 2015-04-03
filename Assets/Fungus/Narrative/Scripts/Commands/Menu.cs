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
	             "Displays a multiple choice menu")]
	[AddComponentMenu("")]
	public class Menu : Command, ILocalizable
	{
		
		// Menu displays a menu button which will execute the target block when clicked
		// Menu Timeout executes a block if the timeout expires
		// The 'Hide If Visited' option checks the execution count of the target block
		// Hide Say dialog when finished? Let Say command handle that
		// Can wrap in an If statement if you need a conditional option

		public string text = "Option Text";

		[Tooltip("Notes about the option text for other authors, localization, etc.")]
		public string description = "";

		[FormerlySerializedAs("targetSequence")]
		public Block targetBlock;

		public bool hideIfVisited;

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

		public override bool RunSlowInEditor()
		{
			return false;
		}

		// ILocalizable methods
		
		public virtual string GetLocalizationID()
		{
			return "MENU." + itemId.ToString(); 
		}
		
		public virtual string GetStandardText()
		{
			return text; 
		}

		public virtual void SetStandardText(string standardText)
		{
			text = standardText;
		}

		public virtual string GetTimestamp()
		{
			return DateTime.Now.ToShortDateString();
		}
		
		public virtual string GetDescription()
		{
			return description;
		}
	}

}