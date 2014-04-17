using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 *  Command classes have their own namespace to prevent them popping up in code completion.
	 */
	namespace Command
	{
		/** 
		 * Fades a sprite to a given alpha value over a period of time
		 */
		public class FadeSprite : CommandQueue.Command
		{
			SpriteRenderer spriteRenderer;
			Color targetColor;
			float fadeDuration;
			Vector2 slideOffset = Vector2.zero;
			
			public FadeSprite(SpriteRenderer _spriteRenderer,
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
		public class SetAnimatorTrigger : CommandQueue.Command
		{
			Animator animator;
			string triggerName;
			
			public SetAnimatorTrigger(Animator _animator,
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
		 * Display a button and set the method to be called when player clicks.
		 */
		public class ShowButton : CommandQueue.Command
		{
			Button button;
			bool visible;
			Action buttonAction;
			
			public ShowButton(Button _button,
			                  bool _visible,
			                  Action _buttonAction)
			{
				if (_button == null)
				{
					Debug.LogError("Button must not be null.");
					return;
				}
				
				button = _button;
				visible = _visible;
				buttonAction = _buttonAction;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				button.Show(visible, buttonAction);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
	}
}