using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus
{

	[CommandInfo("UI",
	             "Set Slider Value",
	             "Sets the value property of a slider object")]
	public class SetSliderValue : Command 
	{
		[Tooltip("Target slider object to set the value on")]
		public Slider slider;

		[Tooltip("Float value to set the slider value to.")]
		public FloatData value;

		public override void OnEnter() 
		{
			slider.value = value;

			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

		public override string GetSummary()
		{
			if (slider == null)
			{
				return "Error: Slider object not selected";
			}

			return slider.name + " = " + value.GetDescription();
		}
	}

}