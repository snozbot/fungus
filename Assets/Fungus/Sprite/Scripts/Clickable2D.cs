using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Fungus
{
	/**
	 * Detects mouse clicks and touches on a Game Object, and sends an event to all Flowchart event handlers in the scene.
	 * The Game Object must have a Collider2D component attached.
	 * Use in conjunction with the ObjectClicked Flowchart event handler.
	 */
	public class Clickable2D : MonoBehaviour 
	{
		[Tooltip("Is object clicking enabled")]
		public bool clickEnabled = true;

		void OnMouseDown()
		{
			if (!clickEnabled)
			{
				return;
			}

			// TODO: Cache these object for faster lookup
			ObjectClicked[] handlers = GameObject.FindObjectsOfType<ObjectClicked>();
			foreach (ObjectClicked handler in handlers)
			{
				handler.OnObjectClicked(this);
			}
		}
	}

}