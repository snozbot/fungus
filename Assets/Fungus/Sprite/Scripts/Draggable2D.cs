using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

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

		[Tooltip("Move object back to its starting position when drag is cancelled")]
		[FormerlySerializedAs("returnToStartPos")]
		public bool returnOnCancelled = true;

		[Tooltip("Move object back to its starting position when drag is completed")]
		public bool returnOnCompleted = true;

		[Tooltip("Time object takes to return to its starting position")]
		public float returnDuration = 1f;

		[Tooltip("Mouse texture to use when hovering mouse over object")]
		public Texture2D hoverCursor;

		protected Vector3 startingPosition;
		protected bool updatePosition = false;
		protected Vector3 newPosition;

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

			newPosition = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10f));
			newPosition.z = z;
			updatePosition = true;
		}

		protected virtual void LateUpdate()
		{
			// iTween will sometimes override the object position even if it should only be affecting the scale, rotation, etc.
			// To make sure this doesn't happen, we force the position change to happen in LateUpdate.
			if (updatePosition)
			{
				transform.position = newPosition;
				updatePosition = false;
			}
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

						if (returnOnCompleted)
						{
							iTween.MoveTo(gameObject, startingPosition, returnDuration);
						}
					}
				}
			}

			if (!dragCompleted)
			{
				foreach (DragCancelled handler in GetHandlers<DragCancelled>())
				{
					handler.OnDragCancelled(this);
				}

				if (returnOnCancelled)
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

		protected virtual void OnMouseEnter()
		{
			changeCursor(hoverCursor);
		}
		
		protected virtual void OnMouseExit()
		{
			SetMouseCursor.ResetMouseCursor();
		}
		
		protected virtual void changeCursor(Texture2D cursorTexture)
		{
			if (!dragEnabled)
			{
				return;
			}
			
			Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
		}
	}

}
