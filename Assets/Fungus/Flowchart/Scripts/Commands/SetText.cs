using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Set Text", 
	             "Sets the text property on a UI Text object.")]

	[AddComponentMenu("")]
	public class SetText : Command 
	{
		[Tooltip("Text object to set text on")]
		public Text textObject;

		[Tooltip("String value to assign to the text object")]
		public StringData stringData;

		public override void OnEnter()
		{
			if (textObject != null)
			{
				textObject.text = stringData.Value;
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (textObject == null)
			{
				return "Error: No text object selected";
			}

			return textObject.name + " : " + stringData.Value;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}