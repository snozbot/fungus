using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Random Integer", 
	             "Sets an integer variable to a random value in the defined range.")]
	public class RandomInteger : Command 
	{
		[Tooltip("The variable whos value will be set")]
		public IntegerVariable variable;

		[Tooltip("Minimum value for random range")]
		public IntegerData minValue;

		[Tooltip("Maximum value for random range")]
		public IntegerData maxValue;

		public override void OnEnter()
		{
			if (variable != null)
			{
				variable.Value = Random.Range(minValue.Value, maxValue.Value);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (variable == null)
			{
				return "Error: Variable not selected";
			}

			return variable.key;
		}

		public override bool HasReference(Variable variable)
		{
			return (variable == this.variable);
		}

		public override Color GetButtonColor()
		{
			return new Color32(253, 253, 150, 255);
		}
	}

}