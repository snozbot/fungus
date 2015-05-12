using UnityEngine;
using System.Collections;

namespace Fungus
{
	// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
	// a Send Message command for sending messages to trigger block execution.

	[CommandInfo("Scripting", 
	             "Spawn Object", 
	             "Spawns a new object based on a reference to a scene or prefab game object.")]
	[AddComponentMenu("")]
	public class SpawnObject : Command
	{
		[Tooltip("Game object to copy when spawning. Can be a scene object or a prefab.")]
		public GameObject sourceObject;

		[Tooltip("Transform to use for position of newly spawned object.")]
		public Transform parentTransform;

		[Tooltip("Local position of newly spawned object.")]
		public Vector3 spawnPosition;

		[Tooltip("Local rotation of newly spawned object.")]
		public Vector3 spawnRotation;

		public override void OnEnter()
		{
			if (sourceObject == null)
			{
				Continue();
				return;
			}

			GameObject newObject = GameObject.Instantiate(sourceObject);
			if (parentTransform != null)
			{
				newObject.transform.parent = parentTransform;
			}

			newObject.transform.localPosition = spawnPosition;
			newObject.transform.localRotation = Quaternion.Euler(spawnRotation);

			Continue();
		}

		public override string GetSummary()
		{
			if (sourceObject == null)
			{
				return "Error: No source GameObject specified";
			}

			return sourceObject.name;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}