using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/** 
	 * Call a delegate method on execution.
	 * This command can be used to schedule arbitrary script code.
	 */
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

			// Execute next command
			onComplete();
		}		
	}

	/**
	 * Wait for a period of time.
	 */
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

	/** 
	 * Sets the currently active view immediately.
	 * The main camera snaps to the active view.
	 */
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
			Game game = Game.GetInstance();

			game.cameraController.SnapToView(view);
			game.activeView = view;

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

	/**
	 * Sets the currently active page for text rendering.
	 */
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

	/**
	 * Sets the currently active Page Style for rendering Pages.
	 */
	public class SetPageStyleCommand : CommandQueue.Command
	{
		PageStyle pageStyle;
		
		public SetPageStyleCommand(PageStyle _pageStyle)
		{
			pageStyle = _pageStyle;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game.GetInstance().activePageStyle = pageStyle;
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}
	
	/**
	 * Sets the title text displayed at the top of the active page.
	 */
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

	/** 
	 * Writes story text to the currently active page.
	 * A 'continue' button is displayed when the text has fully appeared.
	 */
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

	/** 
	 * Adds an option button to the current list of options.
	 * Use the Choose command to display added options.
	 */
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

	/**
	 * Displays all previously added options.
	 */
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

	/** 
	 * Changes the active room to a different room
	 */
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

	/** 
	 * Sets a global boolean flag value
	 */
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
			Game.GetInstance().state.SetFlag(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Sets a global integer counter value
	 */
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
			Game.GetInstance().state.SetCounter(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/**
	 * Sets a global inventory count value
	 */
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
			Game.GetInstance().state.SetInventory(key, value);
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Fades a sprite to a given alpha value over a period of time
	 */
	public class FadeSpriteCommand : CommandQueue.Command
	{
		SpriteRenderer spriteRenderer;
		Color targetColor;
		float fadeDuration;
		Vector2 slideOffset = Vector2.zero;
		
		public FadeSpriteCommand(SpriteRenderer _spriteRenderer,
		                         Color _targetColor,
		                         float _fadeDuration,
		                         Vector2 _slideOffset)
		{
			if (_spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null.");
				return;
			}

			spriteRenderer = _spriteRenderer;
			targetColor = _targetColor;
			fadeDuration = _fadeDuration;
			slideOffset = _slideOffset;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			SpriteFader.FadeSprite(spriteRenderer, targetColor, fadeDuration, slideOffset);

			// Fade is asynchronous, but command completes immediately.
			// If you need to wait for the fade to complete, just use an additional Wait() command
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Sets an animator trigger to change the animator state for an animated sprite
	 */
	public class SetAnimatorTriggerCommand : CommandQueue.Command
	{
		Animator animator;
		string triggerName;

		public SetAnimatorTriggerCommand(Animator _animator,
		                                 string _triggerName)
		{
			if (_animator == null)
			{
				Debug.LogError("Animator must not be null.");
				return;
			}

			animator = _animator;
			triggerName = _triggerName;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			animator.SetTrigger(triggerName);

			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Makes a sprite behave as a clickable button
	 */
	public class AddButtonCommand : CommandQueue.Command
	{
		SpriteRenderer spriteRenderer;
		Action buttonAction;
		
		public AddButtonCommand(SpriteRenderer _spriteRenderer,
		                        Action _buttonAction)
		{
			if (_spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null.");
				return;
			}
			
			spriteRenderer = _spriteRenderer;
			buttonAction = _buttonAction;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Button.MakeButton(spriteRenderer, buttonAction);
			
			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Makes a sprite stop behaving as a clickable button
	 */
	public class RemoveButtonCommand : CommandQueue.Command
	{
		SpriteRenderer spriteRenderer;

		public RemoveButtonCommand(SpriteRenderer _spriteRenderer)
		{
			if (_spriteRenderer == null)
			{
				Debug.LogError("Sprite renderer must not be null.");
				return;
			}
			
			spriteRenderer = _spriteRenderer;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Button button = spriteRenderer.gameObject.GetComponent<Button>();
			GameObject.Destroy(button);

			if (onComplete != null)
			{
				onComplete();
			}
		}		
	}

	/** 
	 * Pans the camera to a view over a period of time.
	 */
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
			Game game = Game.GetInstance();

			game.cameraController.PanToView(view, duration, delegate {

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

	/** 
	 * Pans the camera through a sequence of views over a period of time.
	 */
	public class PanToPathCommand : CommandQueue.Command
	{
		View[] views;
		float duration;
		
		public PanToPathCommand(View[] _views,
		                        	float _duration)
		{
			if (_views.Length == 0)
			{
				Debug.LogError("View list must not be empty.");
				return;
			}
			
			views = _views;
			duration = _duration;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game game = Game.GetInstance();

			game.cameraController.PanToPath(views, duration, delegate {

				if (views.Length > 0)
				{
					game.activeView = views[views.Length - 1];
					
					// Try to find a page that is a child of the active view.
					// If there are multiple child pages then it is the client's responsibility 
					// to set the correct active page in the room script.
					Page defaultPage = game.activeView.gameObject.GetComponentInChildren<Page>();
					if (defaultPage)
					{
						game.activePage = defaultPage;
					}
				}
				
				if (onComplete != null)
				{
					onComplete();
				}
			});
		}		
	}

	/** 
	 * Fades the camera to a view over a period of time.
	 */
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
			Game game = Game.GetInstance();

			game.cameraController.FadeToView(view, duration, delegate {

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

	/** 
	 * Plays a music clip
	 */
	public class PlayMusicCommand : CommandQueue.Command
	{
		AudioClip audioClip;

		public PlayMusicCommand(AudioClip _audioClip)
		{
			if (_audioClip == null)
			{
				Debug.LogError("Audio clip must not be null.");
				return;
			}
			
			audioClip = _audioClip;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game game = Game.GetInstance();

			game.audio.clip = audioClip;
			game.audio.Play();

			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	/** 
	 * Stops a music clip
	 */
	public class StopMusicCommand : CommandQueue.Command
	{
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game game = Game.GetInstance();
			game.audio.Stop();

			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	/** 
	 * Fades music volume to required level over a period of time
	 */
	public class SetMusicVolumeCommand : CommandQueue.Command
	{
		float musicVolume;
		float duration;
		
		public SetMusicVolumeCommand(float _musicVolume, float _duration)
		{
			musicVolume = _musicVolume;
			duration = _duration;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game game = Game.GetInstance();
			iTween.AudioTo(game.gameObject, musicVolume, 1f, duration);

			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	/** 
	 * Plays a sound effect once
	 */
	public class PlaySoundCommand : CommandQueue.Command
	{
		AudioClip audioClip;
		float volume;
		
		public PlaySoundCommand(AudioClip _audioClip, float _volume)
		{
			audioClip = _audioClip;
			volume = _volume;
		}
		
		public override void Execute(CommandQueue commandQueue, Action onComplete)
		{
			Game game = Game.GetInstance();
			game.audio.PlayOneShot(audioClip, volume);

			if (onComplete != null)
			{
				onComplete();
			}
		}
	}
}