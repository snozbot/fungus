using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Animation", 
	             "Set Anim Bool", 
	             "Sets a boolean parameter on an Animator component to control a Unity animation")]
	public class SetAnimBool : Command 
	{
		public Animator animator;
		public string parameterName;
		public BooleanData value;

		public override void OnEnter()
		{
			if (animator != null)
			{
				animator.SetBool(parameterName, value.Value);
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