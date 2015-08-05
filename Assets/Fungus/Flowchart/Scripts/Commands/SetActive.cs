using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Set Active", 
	             "Sets a game object in the scene to be active / inactive.")]
	[AddComponentMenu("")]
	public class SetActive : Command
	{	
		[Tooltip("Reference to game object to enable / disable")]
		public GameObject targetGameObject;

		[Tooltip("Set to true to enable the game object")]
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

			return targetGameObject.name + " = " + activeState.GetDescription();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}