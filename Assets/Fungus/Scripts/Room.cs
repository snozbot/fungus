using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Fungus;

namespace Fungus
{
	// This is the main scripting interface for Fungus games.
	// Each room in your game should have a script which inherits from Room.
	// The OnEnter() method is called when the player enters the room.
	// The OnLeave() method is called when the player moves to a different room.
	// Convenience methods are provided for accessing all features of the library.
	public abstract class Room : MonoBehaviour 
	{
		public int visitCount;

		Game game;
		CommandQueue commandQueue;
		CameraController cameraController;
	 
		void Awake()
		{
			game = Game.GetInstance();
			cameraController = game.gameObject.GetComponent<CameraController>();
			commandQueue = game.gameObject.GetComponent<CommandQueue>();
		}

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
		
		// Internal use only! Called by Game when changing room
		public void Enter()
		{
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

			game.ResetCommandQueue();
			SendMessage("OnEnter", SendMessageOptions.DontRequireReceiver);
			game.ExecuteCommandQueue();

			visitCount++;
		}

		// Internal use only! Called by Game when changing room
		public void Leave()
		{
			SendMessage("OnLeave", SendMessageOptions.DontRequireReceiver);
		}

		// Public convenience methods
		// These methods all execute immediately

		// Returns true if this is the first time the player has visited this room
		public bool IsFirstVisit()
		{
			return (visitCount == 0);
		}

		// Return true if the boolean flag for the key has been set to true
		public bool GetFlag(string key)
		{
			return game.GetFlag(key);
		}

		// Returns the count value for the key
		// Returns zero if no value has been set.
		public int GetCounter(string key)
		{
			return game.GetCounter(key);
		}

		// Returns the inventory count value for the key
		// Returns zero if no inventory count has been set.
		public int GetInventory(string key)
		{
			return game.GetInventory(key);
		}

		// Returns true if the inventory count for the key is greater than zero
		public bool HasInventory(string key)
		{
			return (game.GetInventory(key) > 0);
		}

		// Public command methods
		// These methods all queue commands for later execution in serial order

		// Wait for a period of time before executing the next command
		public void Wait(float duration)
		{
			commandQueue.AddCommand(new WaitCommand(duration));
		}

		// Call a delegate method provided by the client
		// Used to queue the execution of arbitrary code.
		public void Call(Action callAction)
		{
			commandQueue.AddCommand(new CallCommand(callAction));
		}

		// Sets the currently active view immediately.
		// The main camera snaps to the active view.
		public void SetView(View view)
		{
			commandQueue.AddCommand(new SetViewCommand(view));
		}

		// Sets the currently active page for text rendering
		public void SetPage(Page page)
		{
			commandQueue.AddCommand(new SetPageCommand(page));
		}

		// Sets the title text displayed at the top of the active page
		public void Title(string titleText)
		{
			commandQueue.AddCommand(new TitleCommand(titleText));
		}

		// Writes story text to the currently active page.
		// A 'continue' button is displayed when the text has fully appeared.
		public void Say(string storyText)
		{
			commandQueue.AddCommand(new SayCommand(storyText));
		}

		// Adds an option button to the current list of options.
		// Use the Choose command to display added options.
		public void AddOption(string optionText, Action optionAction)
		{
			commandQueue.AddCommand(new AddOptionCommand(optionText, optionAction));
		}

		// Displays a text prompt, followed by all previously added options as buttons.
		public void Choose(string chooseText)
		{
			commandQueue.AddCommand(new ChooseCommand(chooseText));
		}

		// Changes the active room to a different room
		public void MoveToRoom(Room room)
		{
			commandQueue.AddCommand(new MoveToRoomCommand(room));
		}

		// Sets a global boolean flag value
		public void SetFlag(string key, bool value)
		{
			commandQueue.AddCommand(new SetFlagCommand(key, value));
		}

		// Sets a global integer counter value
		public void SetCounter(string key, int value)
		{
			commandQueue.AddCommand(new SetCounterCommand(key, value));
		}

		// Sets a global inventory count value
		// Assumes that the count value is 1 (common case)
		public void SetInventory(string key)
		{
			commandQueue.AddCommand(new SetInventoryCommand(key, 1));
		}

		// Sets a global inventory count value
		public void SetInventory(string key, int value)
		{
			commandQueue.AddCommand(new SetInventoryCommand(key, value));
		}

		// Sets sprite alpha to 0 immediately
		public void HideSprite(SpriteController spriteController)
		{
			commandQueue.AddCommand(new FadeSpriteCommand(spriteController, 0f, 0f, Vector2.zero));
		}

		// Sets sprite alpha to 1 immediately
		public void ShowSprite(SpriteController spriteController)
		{
			commandQueue.AddCommand(new FadeSpriteCommand(spriteController, 1f, 0f, Vector2.zero));
		}

		// Fades a sprite to a given alpha value over a period of time
		public void FadeSprite(SpriteController spriteController, float targetAlpha, float duration)
		{
			commandQueue.AddCommand(new FadeSpriteCommand(spriteController, targetAlpha, duration, Vector2.zero));
		}

		// Fades a sprite to a given alpha value over a period of time, and applies a sliding motion to the sprite transform
		public void FadeSprite(SpriteController spriteController, float targetAlpha, float duration, Vector2 slideOffset)
		{
			commandQueue.AddCommand(new FadeSpriteCommand(spriteController, targetAlpha, duration, slideOffset));
		}

		// Plays the named animation on a object with a SpriteController component
		public void PlayAnimation(SpriteController spriteController, string animationName)
		{
			commandQueue.AddCommand(new PlayAnimationCommand(spriteController, animationName));
		}

		// Pans the camera to the target view of a period of time
		public void PanToView(View targetView, float duration)
		{
			commandQueue.AddCommand(new PanToViewCommand(targetView, duration));
		}

		// Snaps the camera to the target view immediately
		public void SnapToView(View targetView)
		{
			commandQueue.AddCommand(new PanToViewCommand(targetView, 0f));
		}

		// Fades out the current camera view, and fades in again using the target view.
		public void FadeToView(View targetView, float duration)
		{
			commandQueue.AddCommand(new FadeToViewCommand(targetView, duration));
		}
	}
}