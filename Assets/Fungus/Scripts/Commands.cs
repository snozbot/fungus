using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	// Call a delegate method on execution.
	// This command can be used to schedule arbitrary script code.
	public class CallCommand : CommandQueue.Command
	{
		Action callAction;

		public CallCommand(Action _callAction)
		{
			if (_callAction == null)
			{
				Debug.LogError("Action must not be null.");
				return;
			}

			callAction = _callAction;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			if (callAction != null)
			{
				callAction();
			}
			onComplete();
		}		
	}

	// Wait for a period of time
	public class WaitCommand : CommandQueue.Command
	{
		float duration;
		
		public WaitCommand(float _duration)
		{
			duration = _duration;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			commandQueue.StartCoroutine(WaitCoroutine(duration, onComplete));
		}
		
		IEnumerator WaitCoroutine(float duration, Action onComplete)
		{
			yield return new WaitForSeconds(duration);
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	// Sets the currently active view immediately.
	// The main camera snaps to the active view.
	public class SetViewCommand : CommandQueue.Command
	{
		View view;
		
		public SetViewCommand(View _view)
		{
			if (_view == null)
			{
				Debug.LogError("View must not be null");
			}

			view = _view;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			commandQueue.cameraController.SnapToView(view);
			Game.GetInstance().activeView = view;

			// Set the first page component found (if any) as the active page
			Page page = view.gameObject.GetComponentInChildren<Page>();
			if (page != null)
			{
				Game.GetInstance().activePage = page;
			}

			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Sets the currently active page for text rendering
	public class SetPageCommand : CommandQueue.Command
	{
		Page page;
		
		public SetPageCommand(Page _page)
		{
			page = _page;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().activePage = page;
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Sets the title text displayed at the top of the active page
	public class TitleCommand : CommandQueue.Command
	{
		string titleText;
		
		public TitleCommand(string _titleText)
		{
			titleText = _titleText;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Page page = Game.GetInstance().activePage;
			if (page == null)
			{
				Debug.LogError("Active page must not be null");
			}
			else
			{
				page.SetTitle(titleText);
			}
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Writes story text to the currently active page.
	// A 'continue' button is displayed when the text has fully appeared.
	public class SayCommand : CommandQueue.Command
	{
		string storyText;
		
		public SayCommand(string _storyText)
		{
			storyText = _storyText;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Page page = Game.GetInstance().activePage;
			if (page == null)
			{
				Debug.LogError("Active page must not be null");
			}
			else
			{
				page.Say(storyText, onComplete);
			}
		}
	}

	// Adds an option button to the current list of options.
	// Use the Choose command to display added options.
	public class AddOptionCommand : CommandQueue.Command
	{
		string optionText;
		Action optionAction;
		
		public AddOptionCommand(string _optionText, Action _optionAction)
		{
			optionText = _optionText;
			optionAction = _optionAction;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Page page = Game.GetInstance().activePage;
			if (page == null)
			{
				Debug.LogError("Active page must not be null");
			}
			else
			{
				page.AddOption(optionText, optionAction);
			}
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Displays all previously added options.
	public class ChooseCommand : CommandQueue.Command
	{
		string chooseText;

		public ChooseCommand(string _chooseText)
		{
			chooseText = _chooseText;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Page page = Game.GetInstance().activePage;
			if (page == null)
			{
				Debug.LogError("Active page must not be null");
			}
			else
			{
				page.Choose(chooseText);
			}
			// Choose always clears commandQueue, so no need to call onComplete()
		}		
	}

	// Changes the active room to a different room
	public class MoveToRoomCommand : CommandQueue.Command
	{
		Room room;

		public MoveToRoomCommand(Room _room)
		{
			if (_room == null)
			{
				Debug.LogError("Room must not be null.");
				return;
			}

			room = _room;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().MoveToRoom(room);

			// MoveToRoom always resets the command queue so no need to call onComplete
		}
	}

	// Sets a global boolean flag value
	public class SetFlagCommand : CommandQueue.Command
	{
		string key;
		bool value;

		public SetFlagCommand(string _key, bool _value)
		{
			key = _key;
			value = _value;
		}

		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().SetFlag(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Sets a global integer counter value
	public class SetCounterCommand : CommandQueue.Command
	{
		string key;
		int value;
		
		public SetCounterCommand(string _key, int _value)
		{
			key = _key;
			value = _value;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().SetCounter(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Sets a global inventory count value
	public class SetInventoryCommand : CommandQueue.Command
	{
		string key;
		int value;
		
		public SetInventoryCommand(string _key, int _value)
		{
			key = _key;
			value = _value;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().SetInventory(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Fades a sprite to a given alpha value over a period of time
	public class FadeSpriteCommand : CommandQueue.Command
	{
		SpriteController spriteController;
		float targetAlpha;
		float fadeDuration;
		Vector2 slideOffset = Vector2.zero;
		
		public FadeSpriteCommand(SpriteController _spriteController,
		                         float _targetAlpha,
		                         float _fadeDuration,
		                         Vector2 _slideOffset)
		{
			if (_spriteController == null)
			{
				Debug.LogError("Sprite controller must not be null.");
				return;
			}

			spriteController = _spriteController;
			targetAlpha = _targetAlpha;
			fadeDuration = _fadeDuration;
			slideOffset = _slideOffset;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			spriteController.SlideFade(targetAlpha, fadeDuration, slideOffset);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Plays the named animation on a object with a SpriteController component
	public class PlayAnimationCommand : CommandQueue.Command
	{
		SpriteController spriteController;
		string animationName;

		public PlayAnimationCommand(SpriteController _spriteController,
		                            string _animationName)
		{
			if (_spriteController == null)
			{
				Debug.LogError("Sprite controller must not be null.");
				return;
			}

			spriteController = _spriteController;
			animationName = _animationName;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			spriteController.PlayAnimation(animationName);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	// Pans the camera to a view over a period of time.
	public class PanToViewCommand : CommandQueue.Command
	{
		View view;
		float duration;

		public PanToViewCommand(View _view,
		                        float _duration)
		{
			if (_view == null)
			{
				Debug.LogError("View must not be null.");
				return;
			}

			view = _view;
			duration = _duration;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			commandQueue.cameraController.PanToView(view, duration, delegate {

				Game game = Game.GetInstance();
				game.activeView = view;

				// Try to find a page that is a child of the active view.
				// If there are multiple child pages then it is the client's responsibility 
				// to set the correct active page in the room script.
				Page defaultPage = view.gameObject.GetComponentInChildren<Page>();
				if (defaultPage)
				{
					game.activePage = defaultPage;
				}

				if (onComplete != null)
				{
					onComplete();
				}
			});
		}		
	}

	// Fades the camera to a view over a period of time.
	public class FadeToViewCommand : CommandQueue.Command
	{
		View view;
		float duration;

		public FadeToViewCommand(View _view,
		                         float _duration)
		{
			if (_view == null)
			{
				Debug.LogError("View must not be null.");
				return;
			}

			view = _view;
			duration = _duration;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			commandQueue.cameraController.FadeToView(view, duration, delegate {

				Game game = Game.GetInstance();
				game.activeView = view;
				
				// Try to find a page that is a child of the active view.
				// If there are multiple child pages then it is the client's responsibility 
				// to set the correct active page in the room script.
				Page defaultPage = view.gameObject.GetComponentInChildren<Page>();
				if (defaultPage)
				{
					game.activePage = defaultPage;
				}

				if (onComplete != null)
				{
					onComplete();
				}
			});
		}		
	}
}