using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Menu", 
	             "Displays a multiple choice menu")]
	[AddComponentMenu("")]
	public class Menu : Command 
	{
		
		// Menu displays a menu button which will execute the target sequence when clicked
		// Menu Timeout executes a sequence if the timeout expires
		// The 'Hide If Visited' option checks the execution count of the target sequence
		// Hide Say dialog when finished? Let Say command handle that
		// Can wrap in an If statement if you need a conditional option

		public string text = "Option Text";
		public Sequence targetSequence;
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

			bool hideOption = (hideIfVisited && targetSequence != null && targetSequence.GetExecutionCount() > 0);

			if (!hideOption)
			{
				MenuDialog menuDialog = MenuDialog.GetMenuDialog();
				if (menuDialog != null)
				{
					menuDialog.gameObject.SetActive(true);
					string displayText = text;
					menuDialog.AddOption(displayText, targetSequence);
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
				GameObject go = Resources.Load<GameObject>("FungusEventSystem");
				if (go != null)
				{
					GameObject spawnedGO = Instantiate(go) as GameObject;
					spawnedGO.name = "EventSystem";
				}
			}

			eventSystemPresent = true;
		}

		public override void GetConnectedSequences(ref List<Sequence> connectedSequences)
		{
			if (targetSequence != null)
			{
				connectedSequences.Add(targetSequence);
			}		
		}

		public override string GetSummary()
		{
			if (targetSequence == null)
			{
				return "Error: No target sequence selected";
			}

			if (text == "")
			{
				return "Error: No button text selected";
			}

			return text + " : " + targetSequence.sequenceName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(184, 210, 235, 255);
		}

		public override bool RunSlowInEditor()
		{
			return false;
		}
	}

}