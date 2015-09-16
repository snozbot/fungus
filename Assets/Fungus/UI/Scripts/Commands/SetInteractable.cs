using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CommandInfo("UI", 
	             "Set Interactable", 
	             "Set the interactable sate of selectable objects.")]
	public class SetInteractable : Command 
	{
		[Tooltip("List of objects to be affected by the command")]
		public List<GameObject> targetObjects = new List<GameObject>();

		[Tooltip("Controls if the selectable UI object be interactable or not")]
		public BooleanData interactableState = new BooleanData(true);

		public override void OnEnter()
		{
			if (targetObjects.Count == 0)
			{
				Continue();
				return;
			}

			foreach (GameObject targetObject in targetObjects)
			{
				foreach (Selectable selectable in targetObject.GetComponents<Selectable>())
				{
					selectable.interactable = interactableState.Value;
				}
			}
				
			Continue();
		}

		public override string GetSummary()
		{
			if (targetObjects.Count == 0)
			{
				return "Error: No targetObjects selected";
			}
			else if (targetObjects.Count == 1)
			{
				if (targetObjects[0] == null)
				{
					return "Error: No targetObjects selected";
				}
				return targetObjects[0].name + " = " + interactableState.Value;
			}
			
			string objectList = "";
			foreach (GameObject gameObject in targetObjects)
			{
				if (gameObject == null)
				{
					continue;
				}
				
				if (objectList == "")
				{
					objectList += gameObject.name;
				}
				else
				{
					objectList += ", " + gameObject.name;
				}
			}
			
			return objectList + " = " + interactableState.Value;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(180, 250, 250, 255);
		}

		public override void OnCommandAdded(Block parentBlock)
		{
			targetObjects.Add(null);
		}

		public override bool IsReorderableArray(string propertyName)
		{
			if (propertyName == "targetObjects")
			{
				return true;
			}

			return false;
		}
	}

}