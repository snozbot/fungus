using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Dialog", 
	             "Menu Timer", 
	             "Displays a timer bar and executes a target sequence if the player fails to select a menu option in time.")]
	[AddComponentMenu("")]
	public class MenuTimer : Command 
	{		
		public float duration;
		public Sequence targetSequence;

		public override void OnEnter()
		{
			MenuDialog menuDialog = MenuDialog.GetMenuDialog();

			if (menuDialog != null &&
			    targetSequence != null)
			{
				menuDialog.ShowTimer(duration, targetSequence);
			}

			Continue();
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

			return targetSequence.sequenceName;
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