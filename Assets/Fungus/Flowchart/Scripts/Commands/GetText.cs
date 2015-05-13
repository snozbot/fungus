using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Get Text", 
	             "Gets the text property from a UI Text object and stores it in a string variable.")]

	[AddComponentMenu("")]
	public class GetText : Command 
	{
		[Tooltip("Text object to get text value from")]
		public Text textObject;

		[Tooltip("String variable to store the text value in")]
		[VariableProperty(typeof(StringVariable))]
		public Variable variable;

		public override void OnEnter()
		{
			if (textObject != null)
			{
				StringVariable stringVariable = variable as StringVariable;
				if (stringVariable != null)
				{
					stringVariable.value = textObject.text;
				}
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (textObject == null)
			{
				return "Error: No text object selected";
			}

			if (variable == null)
			{
				return "Error: No variable selected";
			}

			return textObject.name + " : " + variable.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}