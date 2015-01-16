using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Animation", 
	             "Set Anim Trigger", 
	             "Sets a trigger parameter on an Animator component to control a Unity animation")]
	[AddComponentMenu("")]
	public class SetAnimTrigger : Command 
	{
		[Tooltip("Reference to an Animator component in a game object")]
		public Animator animator;

		[Tooltip("Name of the trigger Animator parameter that will have its value changed")]
		public string parameterName;

		public override void OnEnter()
		{
			if (animator != null)
			{
				animator.SetTrigger(parameterName);
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