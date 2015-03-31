using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Set Clickable 2D", 
	             "Sets a Clickable2D component to be clickable / non-clickable.")]
	[AddComponentMenu("")]
	public class SetClickable2D : Command
	{       
		[Tooltip("Reference to Clickable2D component on a gameobject")]
		public Clickable2D targetClickable2D;

		[Tooltip("Set to true to enable the component")]
		public BooleanData activeState;

		public override void OnEnter()	
		{
			if (targetClickable2D != null)		
			{
				targetClickable2D.clickEnabled = activeState.Value;	
			}
			
			Continue();
		}
		
		public override string GetSummary()
		{
			if (targetClickable2D == null)		
			{
				return "Error: No Clickable2D component selected";	
			}
			
			return targetClickable2D.gameObject.name;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);	
		}
	}
		
}