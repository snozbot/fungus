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
				
				game.cameraController.PanToPosition(view.transform.position, view.viewSize, 0, null);
				
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
			float targetSize;
			float duration;

			public PanToPosition(Vector3 _targetPosition, float _targetSize, float _duration)
			{
				targetPosition = _targetPosition;
				targetSize = _targetSize;
				duration = _duration;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.waiting = true;
				
				game.cameraController.PanToPosition(targetPosition, targetSize, duration, delegate {
					
					game.waiting = false;
					
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
		 * Switches on manual pan mode.
		 * This allows the player to swipe the screen to pan around a region defined by 2 views.
		 */
		public class StartManualPan : CommandQueue.Command
		{
			View viewA;
			View viewB;
			float duration;
			
			public StartManualPan(View _viewA, View _viewB, float _duration)
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

				game.cameraController.StartManualPan(viewA, viewB, duration, delegate {

					game.waiting = false;

					if (onComplete != null)
					{
						onComplete();
					}
				});
			}		
		}
		
		/** 
		 * Switches off manual pan mode.
		 */
		public class StopManualPan : CommandQueue.Command
		{
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.cameraController.StopManualPan();
				
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