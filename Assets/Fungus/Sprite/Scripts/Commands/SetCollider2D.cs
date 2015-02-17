using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Set Collider2D", 
	             "Sets a Collider2D component to be active / inactive. Use this to make a clickable object be non-clickable.")]
	[AddComponentMenu("")]
	public class SetCollider2D : Command
	{       
		[Tooltip("Reference to Collider2D component on a gameobject")]
		public Collider2D targetCollider2D;

		[Tooltip("Set to true to enable the component")]
		public BooleanData activeState;

		public override void OnEnter()	
		{
			if (targetCollider2D != null)		
			{
				targetCollider2D.enabled = activeState.Value;	
			}
			
			Continue();
		}
		
		public override string GetSummary()
		{
			if (targetCollider2D == null)		
			{
				return "Error: No Collider2D component selected";	
			}
			
			return targetCollider2D.gameObject.name;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);	
		}
	}
		
}