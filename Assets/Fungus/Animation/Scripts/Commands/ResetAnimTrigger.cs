using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Animation", 
	             "Reset Anim Trigger", 
	             "Resets a trigger parameter on an Animator component.")]
	[AddComponentMenu("")]
	public class ResetAnimTrigger : Command 
	{
		[Tooltip("Reference to an Animator component in a game object")]
		public Animator animator;

		[Tooltip("Name of the trigger Animator parameter that will be reset")]
		public string parameterName;

		public override void OnEnter()
		{
			if (animator != null)
			{
				animator.ResetTrigger(parameterName);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (animator == null)
			{
				return "Error: No animator selected";
			}

			return animator.name + " (" + parameterName + ")";
		}

		public override Color GetButtonColor()
		{
			return new Color32(170, 204, 169, 255);
		}
	}

}