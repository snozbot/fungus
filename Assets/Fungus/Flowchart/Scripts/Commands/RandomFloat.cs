/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Variable", 
	             "Random Float", 
	             "Sets an float variable to a random value in the defined range.")]
	[AddComponentMenu("")]
	public class RandomFloat : Command 
	{
		[Tooltip("The variable whos value will be set")]
		[VariableProperty(typeof(FloatVariable))]
		public FloatVariable variable;

		[Tooltip("Minimum value for random range")]
		public FloatData minValue;

		[Tooltip("Maximum value for random range")]
		public FloatData maxValue;

		public override void OnEnter()
		{
			if (variable != null)
			{
				variable.value = Random.Range(minValue.Value, maxValue.Value);
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