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
		 * Sets the currently active view immediately.
		 * The main camera snaps to the active view.
		 */
		public class SetView : CommandQueue.Command
		{
			View view;
			
			public SetView(View _view)
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
				
				game.cameraController.PanToPosition(view.transform.position, view.transform.rotation, view.viewSize, 0, null);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}

		/** 
		 * Pans the camera to a target position & size over a period of time.
		 */
		public class PanToPosition : CommandQueue.Command
		{
			Vector3 targetPosition;
			Quaternion targetRotation;
			float targetSize;
			float duration;
			bool wait;

			public PanToPosition(Vector3 _targetPosition, Quaternion _targetRotation, float _targetSize, float _duration, bool _wait)
			{
				targetPosition = _targetPosition;
				targetRotation = _targetRotation;
				targetSize = _targetSize;
				duration = _duration;
				wait = _wait;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();

				if (wait)
				{
					game.waiting = true;
				}

				game.cameraController.PanToPosition(targetPosition, targetRotation, targetSize, duration, delegate {
					if (wait)
					{
						game.waiting = false;
						if (onComplete != null)
						{
							onComplete();
						}
					}
				});

				if (!wait)
				{
					if (onComplete != null)
					{
						onComplete();
					}
				}
			}		
		}

		/** 
		 * Pans the camera through a sequence of views over a period of time.
		 */
		public class PanToPath : CommandQueue.Command
		{
			View[] views;
			float duration;
			
			public PanToPath(View[] _views, float _duration)
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
				
				game.waiting = true;
				
				game.cameraController.PanToPath(views, duration, delegate {
					
					if (views.Length > 0)
					{
						game.waiting = false;
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
		public class FadeToView : CommandQueue.Command
		{
			View view;
			float duration;
			
			public FadeToView(View _view, float _duration)
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
				
				game.waiting = true;
				
				game.cameraController.FadeToView(view, duration, delegate {
					
					game.waiting = false;
					
					if (onComplete != null)
					{
						onComplete();
					}
				});
			}		
		}
		
		/** 
		 * Switches on swipe panning mode.
		 * This allows the player to swipe the screen to pan around a region defined by 2 views.
		 */
		public class StartSwipePan : CommandQueue.Command
		{
			View viewA;
			View viewB;
			float duration;
			
			public StartSwipePan(View _viewA, View _viewB, float _duration)
			{
				if (_viewA == null ||
				    _viewB == null)
				{
					Debug.LogError("Views must not be null.");
					return;
				}
				
				viewA = _viewA;
				viewB = _viewB;
				duration = _duration;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();

				game.waiting = true;

				game.cameraController.StartSwipePan(viewA, viewB, duration, delegate {

					game.waiting = false;

					if (onComplete != null)
					{
						onComplete();
					}
				});
			}		
		}
		
		/** 
		 * Switches off pan-to-swipe mode.
		 */
		public class StopSwipePan : CommandQueue.Command
		{
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.cameraController.StopSwipePan();
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/** 
		 * Stores the current camera position
		 */
		public class StoreView : CommandQueue.Command
		{
			string viewName;

			public StoreView(string _viewName)
			{
				viewName = _viewName;
			}

			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.cameraController.StoreView(viewName);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}		
		}
		
		/** 
		 * Pans the camera to a view over a period of time.
		 */
		public class PanToStoredView : CommandQueue.Command
		{
			float duration;
			string viewName;
			
			public PanToStoredView(string _viewName, float _duration)
			{
				viewName = _viewName;
				duration = _duration;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.waiting = true;
				
				game.cameraController.PanToStoredView(viewName, duration, delegate {
					
					game.waiting = false;
					
					if (onComplete != null)
					{
						onComplete();
					}
				});
			}		
		}
	}
}