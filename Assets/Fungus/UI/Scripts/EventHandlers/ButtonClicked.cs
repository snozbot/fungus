using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus 
{
	[EventHandlerInfo("UI",
	                  "Button Clicked",
	                  "The block will execute when the user clicks on the target UI button object.")]
	[AddComponentMenu("")]
	public class ButtonClicked : EventHandler
	{	
		[Tooltip("The UI Button that the user can click on")]
		public Button targetButton;
		
		public virtual void Start()
		{
			if (targetButton != null)
			{
				targetButton.onClick.AddListener(OnButtonClick);
			}
		}
		
		protected virtual void OnButtonClick()
		{
			ExecuteBlock();
		}

		public override string GetSummary()
		{
			if (targetButton != null)
			{
				return targetButton.name;
			}

			return "None";
		}
	}
}
