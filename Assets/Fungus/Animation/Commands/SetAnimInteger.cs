using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Animation", 
	             "Set Anim Integer", 
	             "Sets an integer parameter on an animator to control an animation")]
	public class SetAnimInteger : Command 
	{
		public Animator animator;
		public string parameterName;
		public IntegerData value;

		public override void OnEnter()
		{
			if (animator != null)
			{
				animator.SetInteger(parameterName, value.Value);
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