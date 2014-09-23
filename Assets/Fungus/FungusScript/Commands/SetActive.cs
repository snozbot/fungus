using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Set Active", 
	             "Sets a game object to be active / inactive.")]
	public class SetActive : Command
	{	
		public GameObject targetGameObject;

		public BooleanData activeState;
	
		public override void OnEnter()
		{
			if (targetGameObject != null)
			{
				targetGameObject.SetActive(activeState.Value);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (targetGameObject == null)
			{
				return "Error: No game object selected";
			}

			return targetGameObject.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}