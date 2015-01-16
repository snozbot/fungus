using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Destroy", 
	             "Destroys a specified game object in the scene.")]
	[AddComponentMenu("")]
	public class Destroy : Command
	{	
		[Tooltip("Reference to game object to destroy")]
		public GameObject targetGameObject;

		public override void OnEnter()
		{
			if (targetGameObject != null)
			{
				Destroy(targetGameObject);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (targetGameObject == null)
			{
				return "Error: No game object selected";
			}

			return targetGameObject.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}