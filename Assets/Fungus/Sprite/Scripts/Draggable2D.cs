using UnityEngine;
using System.Collections;

namespace Fungus
{

	/**
	 * Detects drag and drop interactions on a Game Object, and sends events to all Flowchart event handlers in the scene.
	 * The Game Object must have Collider2D & RigidBody components attached. 
	 * The Collider2D must have the Is Trigger property set to true.
	 * The RigidBody would typically have the Is Kinematic property set to true, unless you want the object to move around using physics.
	 * Use in conjunction with the Drag Started, Drag Completed, Drag Cancelled, Drag Entered & Drag Exited event handlers.
	 */
	public class Draggable2D : MonoBehaviour 
	{
		[Tooltip("Is object dragging enabled")]
		public bool dragEnabled = true;

		[Tooltip("Move object back to its starting position when drag is released")]
		public bool returnToStartPos = true;

		[Tooltip("Time object takes to return to its starting position")]
		public float returnDuration = 1f;

		protected Vector3 startingPosition;

		protected virtual void OnMouseDown()
		{
			startingPosition = transform.position;

			foreach (DragStarted handler in GetHandlers<DragStarted>())
			{
				handler.OnDragStarted(this);
			}
		}

		protected virtual void OnMouseDrag()
		{
			if (!dragEnabled)
			{
				return;
			}

			float x = Input.mousePosition.x;
			float y = Input.mousePosition.y;
			float z = transform.position.z;

			Vector3 newPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10f));
			newPosition.z = z;

			transform.position = newPosition;
		}
		
		protected virtual void OnMouseUp()
		{
			if (!dragEnabled)
			{
				return;
			}

			bool dragCompleted = false;

			DragCompleted[] handlers = GetHandlers<DragCompleted>();
			foreach (DragCompleted handler in handlers)
			{
				if (handler.draggableObject == this)
				{
					if (handler.IsOverTarget())
					{
						handler.OnDragCompleted(this);
						dragCompleted = true;
					}
				}
			}

			if (!dragCompleted)
			{
				foreach (DragCancelled handler in GetHandlers<DragCancelled>())
				{
					handler.OnDragCancelled(this);
				}

				if (returnToStartPos)
				{
					iTween.MoveTo(gameObject, startingPosition, returnDuration);
				}
			}

		}

		protected virtual void OnTriggerEnter2D(Collider2D other) 
		{
			if (!dragEnabled)
			{
				return;
			}

			foreach (DragEntered handler in GetHandlers<DragEntered>())
			{
				handler.OnDragEntered(this, other);
			}

			foreach (DragCompleted handler in GetHandlers<DragCompleted>())
			{
				handler.OnDragEntered(this, other);
			}
		}

		protected virtual void OnTriggerExit2D(Collider2D other) 
		{
			if (!dragEnabled)
			{
				return;
			}

			foreach (DragExited handler in GetHandlers<DragExited>())
			{
				handler.OnDragExited(this, other);
			}

			foreach (DragCompleted handler in GetHandlers<DragCompleted>())
			{
				handler.OnDragExited(this, other);
			}
		}

		protected virtual T[] GetHandlers<T>() where T : EventHandler
		{
			// TODO: Cache these object for faster lookup
			return GameObject.FindObjectsOfType<T>();
		}
	}

}
