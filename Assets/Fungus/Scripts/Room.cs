using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Fungus;

namespace Fungus
{
	/**
	 * This is the primary base class for scripting Fungus games.
	 * Each Room in your game should have a script component which inherits from Room.
	 * The OnEnter() method is called when the player enters the room.
	 * The GameController base class provides easy access to all Fungus functionality.
	 */
	public abstract class Room : GameController 
	{
		/**
		 * Number of times player has entered the room
		 */
		public int visitCount;

		/**
		 * Returns true if this is the first time the player has visited this room.
		 */
		public bool IsFirstVisit()
		{
			return (visitCount == 0);
		}

		// Automatically draws arrows to other Rooms referenced in public properties
		void OnDrawGizmos()
		{
			const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
			FieldInfo[] fields = this.GetType().GetFields(flags);

			foreach (FieldInfo fieldInfo in fields)
			{
				Room room = fieldInfo.GetValue(this) as Room;
				if (room != null)
				{
					DrawLinkToRoom(room);
				}
			}
		}

		void DrawLinkToRoom(Room room)
		{
			if (!room)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireCube(transform.position, renderer.bounds.size * 1.1f);
				return;
			}
			
			if (!Game.GetInstance().showLinks)
			{
				return;
			}
			
			Gizmos.color = Color.green;
			Vector3 posA = transform.position;
			Vector3 posB = room.transform.position;
			
			Ray toA = new Ray(posB, posA - posB);
			Ray toB = new Ray(posA, posB - posA);
			
			float tA = 0;
			if (renderer)
			{
				if (renderer.bounds.IntersectRay(toA, out tA))
				{
					posA = toA.GetPoint(tA * 0.95f);
				}
			}
			
			float tB = 0;
			if (room.gameObject.renderer)
			{
				if (room.gameObject.renderer.bounds.IntersectRay(toB, out tB))
				{
					posB = toB.GetPoint(tB * 0.95f);
				}
			}
			
			Gizmos.DrawLine(posA, posB);
			
			float arrowHeadSize = 0.25f;
			
			Vector3 arrowPosA = posB;
			Vector3 arrowPosB = arrowPosA;
			Vector3 arrowPosC = arrowPosA;
			
			arrowPosB.x += toB.direction.y * arrowHeadSize;
			arrowPosB.y -= toB.direction.x * arrowHeadSize;
			arrowPosB -= toB.direction * arrowHeadSize;
			Gizmos.DrawLine(arrowPosA, arrowPosB);
			
			arrowPosC.x -= toB.direction.y * arrowHeadSize;
			arrowPosC.y += toB.direction.x * arrowHeadSize;
			arrowPosC -= toB.direction * arrowHeadSize;
			Gizmos.DrawLine(arrowPosA, arrowPosC);
		}
		
		// Called by Game when player enters the room
		void Enter()
		{
			Game game = Game.GetInstance();
			CameraController cameraController = game.gameObject.GetComponent<CameraController>();


			// Pick first view found in the room and snap to camera to this view.
			// It is allowed for a room to not have any views. 
			// In this case game.activeView will be null, and the camera will attempt
			// to snap to the room sprite.
			View view = gameObject.GetComponentInChildren<View>();
			if (view == null)
			{
				// No view defined for this room, try to center on room sprite
				SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
				if (spriteRenderer != null)
				{
					cameraController.CenterOnSprite(spriteRenderer);
				}
				else
				{
					Debug.LogError("Failed to set camera view when entering room.");
				}
			}
			else
			{
				// Snap to new view
				cameraController.SnapToView(view);
				game.activeView = view;
			}

			// Pick first page found in room
			// It is allowed for a room to not have any pages. In this case game.activePage will be null
			game.activePage = gameObject.GetComponentInChildren<Page>();

			// Rooms may have multiple child views and page. It is the responsibility of the client
			// room script to set the appropriate view & page in its OnEnter method.

			game.commandQueue.CallCommandMethod(game.activeRoom.gameObject, "OnEnter");

			visitCount++;
		}
	}
}