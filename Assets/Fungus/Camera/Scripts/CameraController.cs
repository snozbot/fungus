/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Fungus;

namespace Fungus
{
	/**
	 * Controller for main camera.
	 * Supports several types of camera transition including snap, pan & fade.
	 */
	public class CameraController : MonoBehaviour 
	{
		/**
		 * Full screen texture used for screen fade effect.
		 */
		public Texture2D screenFadeTexture;
		
		/**
		 * Icon to display when swipe pan mode is active.
		 */
		public Texture2D swipePanIcon;
		
		/**
		 * Position of continue and swipe icons in normalized screen space coords.
		 * (0,0) = top left, (1,1) = bottom right
		 */
		public Vector2 swipeIconPosition = new Vector2(1,0);

		/**
		 * Set the camera z coordinate to a fixed value every frame.
		 */
		public bool setCameraZ = true;

		/**
		 * Fixed Z coordinate of main camera.
		 */
		public float cameraZ = -10f;

		[HideInInspector]
		public bool waiting; 
		
		protected float fadeAlpha = 0f;
		
		// Swipe panning control
		[HideInInspector]
		public bool swipePanActive;
		public Camera swipeCamera;

		[HideInInspector]
		public float swipeSpeedMultiplier = 1f;
		protected View swipePanViewA;
		protected View swipePanViewB;
		protected Vector3 previousMousePos;
		
		protected class CameraView
		{
			public Vector3 cameraPos;
			public Quaternion cameraRot;
			public float cameraSize;
		};
		
		protected Dictionary<string, CameraView> storedViews = new Dictionary<string, CameraView>();
		
		protected static CameraController instance;
		
		/**
		 * Returns the CameraController singleton instance.
		 * Will create a CameraController game object if none currently exists.
		 */
		static public CameraController GetInstance()
		{
			if (instance == null)
			{
				GameObject go = new GameObject("CameraController");
				instance = go.AddComponent<CameraController>();
			}
			
			return instance;
		}
		
		public static Texture2D CreateColorTexture(Color color, int width, int height)
		{
			Color[] pixels = new Color[width * height];
			for (int i = 0; i < pixels.Length; i++) 
			{
				pixels[i] = color;
			}
			Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
			texture.SetPixels(pixels);
			texture.Apply();
			
			return texture;		
		}
		
		protected virtual void OnGUI()
		{
			if (swipePanActive)
			{
				// Draw the swipe panning icon
				if (swipePanIcon)
				{
					float x = Screen.width * swipeIconPosition.x;
					float y = Screen.height * swipeIconPosition.y;
					float width = swipePanIcon.width;
					float height = swipePanIcon.height;
					
					x = Mathf.Max(x, 0);
					y = Mathf.Max(y, 0);
					x = Mathf.Min(x, Screen.width - width);
					y = Mathf.Min(y, Screen.height - height);
					
					Rect rect = new Rect(x, y, width, height);
					GUI.DrawTexture(rect, swipePanIcon);
				}
			}
			
			// Draw full screen fade texture
			if (fadeAlpha > 0f &&
			    screenFadeTexture != null)
			{
				// 1 = scene fully visible
				// 0 = scene fully obscured
				GUI.color = new Color(1,1,1, fadeAlpha);	
				GUI.depth = -1000;
				GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), screenFadeTexture);
			}
		}
		
		/**
		 * Perform a fullscreen fade over a duration.
		 */
		public virtual void Fade(float targetAlpha, float fadeDuration, Action fadeAction)
		{
			StartCoroutine(FadeInternal(targetAlpha, fadeDuration, fadeAction));
		}
		
		/**
		 * Fade out, move camera to view and then fade back in.
		 */
		public virtual void FadeToView(Camera camera, View view, float fadeDuration, bool fadeOut, Action fadeAction)
		{
			swipePanActive = false;
			fadeAlpha = 0f;

			float outDuration;
			float inDuration;

			if (fadeOut)
			{
				outDuration = fadeDuration / 2f;
				inDuration = fadeDuration / 2f;
			}
			else
			{
				outDuration = 0;
				inDuration = fadeDuration;
			}

			// Fade out
			Fade(1f, outDuration, delegate {
				
				// Snap to new view
				PanToPosition(camera, view.transform.position, view.transform.rotation, view.viewSize, 0f, null);
				
				// Fade in
				Fade(0f, inDuration, delegate {
					if (fadeAction != null)
					{
						fadeAction();
					}
				});
			});
		}
		
		protected virtual IEnumerator FadeInternal(float targetAlpha, float fadeDuration, Action fadeAction)
		{
			float startAlpha = fadeAlpha;
			float timer = 0;
			
			// If already at the target alpha then complete immediately
			if (startAlpha == targetAlpha)
			{
				yield return null;
			}
			else
			{
				while (timer < fadeDuration)
				{
					float t = timer / fadeDuration;
					timer += Time.deltaTime;
					
					t = Mathf.Clamp01(t);   
					
					fadeAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
					yield return null;
				}
			}
			
			fadeAlpha = targetAlpha;
			
			if (fadeAction != null)
			{
				fadeAction();
			}
		}
		
		/**
		 * Positions camera so sprite is centered and fills the screen.
		 * @param spriteRenderer The sprite to center the camera on
		 */
		public virtual void CenterOnSprite(Camera camera, SpriteRenderer spriteRenderer)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			if (spriteRenderer == null)
			{
				Debug.LogWarning("Sprite renderer is null");
				return;
			}

			swipePanActive = false;
			
			Sprite sprite = spriteRenderer.sprite;
			Vector3 extents = sprite.bounds.extents;
			float localScaleY = spriteRenderer.transform.localScale.y;

			camera.orthographicSize = extents.y * localScaleY;
			Vector3 pos = spriteRenderer.transform.position;
			camera.transform.position = new Vector3(pos.x, pos.y, 0);
	
			SetCameraZ(camera);
		}
		
		public virtual void PanToView(Camera camera, View view, float duration, Action arriveAction)
		{
			PanToPosition(camera, view.transform.position, view.transform.rotation, view.viewSize, duration, arriveAction);
		}
		
		/**
		 * Moves camera from current position to a target position over a period of time.
		 */
		public virtual void PanToPosition(Camera camera, Vector3 targetPosition, Quaternion targetRotation, float targetSize, float duration, Action arriveAction)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			// Stop any pan that is currently active
			StopAllCoroutines();
			
			swipePanActive = false;
			
			if (duration == 0f)
			{
				// Move immediately
				camera.orthographicSize = targetSize;
				camera.transform.position = targetPosition;
				camera.transform.rotation = targetRotation;

				SetCameraZ(camera);

				if (arriveAction != null)
				{
					arriveAction();
				}
			}
			else
			{
				StartCoroutine(PanInternal(camera, targetPosition, targetRotation, targetSize, duration, arriveAction));
			}
		}
		
		/**
		 * Stores the current camera view using a name.
		 */
		public virtual void StoreView(Camera camera, string viewName)
		{
			if (camera != null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			CameraView currentView = new CameraView();
			currentView.cameraPos = camera.transform.position;
			currentView.cameraRot = camera.transform.rotation;
			currentView.cameraSize = camera.orthographicSize;
			storedViews[viewName] = currentView;
		}
		
		/**
		 * Moves the camera to a previously stored camera view over a period of time.
		 */
		public virtual void PanToStoredView(Camera camera, string viewName, float duration, Action arriveAction)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			if (!storedViews.ContainsKey(viewName))
			{
				// View has not previously been stored
				if (arriveAction != null)
				{
					arriveAction();
				}
				return;
			}
			
			CameraView cameraView = storedViews[viewName];
			
			if (duration == 0f)
			{
				// Move immediately
				camera.transform.position = cameraView.cameraPos;
				camera.transform.rotation = cameraView.cameraRot;
				camera.orthographicSize = cameraView.cameraSize;

				SetCameraZ(camera);

				if (arriveAction != null)
				{
					arriveAction();
				}
			}
			else
			{
				StartCoroutine(PanInternal(camera, cameraView.cameraPos, cameraView.cameraRot, cameraView.cameraSize, duration, arriveAction));
			}
		}
		
		protected virtual IEnumerator PanInternal(Camera camera, Vector3 targetPos, Quaternion targetRot, float targetSize, float duration, Action arriveAction)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				yield break;
			}

			float timer = 0;
			float startSize = camera.orthographicSize;
			float endSize = targetSize;
			Vector3 startPos = camera.transform.position;
			Vector3 endPos = targetPos;
			Quaternion startRot = camera.transform.rotation;
			Quaternion endRot = targetRot;
			
			bool arrived = false;
			while (!arrived)
			{
				timer += Time.deltaTime;
				if (timer > duration)
				{
					arrived = true;
					timer = duration;
				}
				
				// Apply smoothed lerp to camera position and orthographic size
				float t = 1f;
				if (duration > 0f)
				{
					t = timer / duration;
				}

				if (camera != null)
				{
					camera.orthographicSize = Mathf.Lerp(startSize, endSize, Mathf.SmoothStep(0f, 1f, t));
					camera.transform.position = Vector3.Lerp(startPos, endPos, Mathf.SmoothStep(0f, 1f, t));
					camera.transform.rotation = Quaternion.Lerp(startRot, endRot, Mathf.SmoothStep(0f, 1f, t));
				}

				SetCameraZ(camera);
				
				if (arrived &&
				    arriveAction != null)
				{
					arriveAction();
				}
				
				yield return null;
			}
		}
		
		/**
		 * Moves camera smoothly through a sequence of Views over a period of time
		 */
		public virtual void PanToPath(Camera camera, View[] viewList, float duration, Action arriveAction)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			swipePanActive = false;
			
			List<Vector3> pathList = new List<Vector3>();
			
			// Add current camera position as first point in path
			// Note: We use the z coord to tween the camera orthographic size
			Vector3 startPos = new Vector3(camera.transform.position.x,
			                               camera.transform.position.y,
			                               camera.orthographicSize);
			pathList.Add(startPos);
			
			for (int i = 0; i < viewList.Length; ++i)
			{
				View view = viewList[i];
				
				Vector3 viewPos = new Vector3(view.transform.position.x, 
				                              view.transform.position.y, 
				                              view.viewSize);
				pathList.Add(viewPos);
			}
			
			StartCoroutine(PanToPathInternal(camera, duration, arriveAction, pathList.ToArray()));
		}
		
		protected virtual IEnumerator PanToPathInternal(Camera camera, float duration, Action arriveAction, Vector3[] path)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				yield break;
			}

			float timer = 0;
			
			while (timer < duration)
			{
				timer += Time.deltaTime;
				timer = Mathf.Min(timer, duration);
				float percent = timer / duration;
				
				Vector3 point = iTween.PointOnPath(path, percent);
				
				camera.transform.position = new Vector3(point.x, point.y, 0);
				camera.orthographicSize = point.z;

				SetCameraZ(camera);
				
				yield return null;
			}
			
			if (arriveAction != null)
			{
				arriveAction();
			}
		}
		
		/**
		 * Activates swipe panning mode.
		 * The player can pan the camera within the area between viewA & viewB.
		 */
		public virtual void StartSwipePan(Camera camera, View viewA, View viewB, float duration, float speedMultiplier, Action arriveAction)
		{
			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			swipePanViewA = viewA;
			swipePanViewB = viewB;
			swipeSpeedMultiplier = speedMultiplier;
			
			Vector3 cameraPos = camera.transform.position;
			
			Vector3 targetPosition = CalcCameraPosition(cameraPos, swipePanViewA, swipePanViewB);
			float targetSize = CalcCameraSize(cameraPos, swipePanViewA, swipePanViewB); 
			
			PanToPosition(camera, targetPosition, Quaternion.identity, targetSize, duration, delegate {
				
				swipePanActive = true;
				swipeCamera = camera;
				
				if (arriveAction != null)
				{
					arriveAction();
				}
			}); 
		}
		
		/**
		 * Deactivates swipe panning mode.
		 */
		public virtual void StopSwipePan()
		{
			swipePanActive = false;
			swipePanViewA = null;
			swipePanViewB = null;
			swipeCamera = null;
		}
		
		protected virtual void SetCameraZ(Camera camera)
		{
			if (!setCameraZ)
			{
				return;
			}

			if (camera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, cameraZ);
		}
		
		protected virtual void Update()	
		{
			if (!swipePanActive)
			{
				return;
			}

			if (swipeCamera == null)
			{
				Debug.LogWarning("Camera is null");
				return;
			}

			Vector3 delta = Vector3.zero;
			
			if (Input.touchCount > 0)
			{
				if (Input.GetTouch(0).phase == TouchPhase.Moved)
				{
					delta = Input.GetTouch(0).deltaPosition;
				}
			}
			
			if (Input.GetMouseButtonDown(0))
			{
				previousMousePos = Input.mousePosition;	
			}
			else if (Input.GetMouseButton(0)) 
			{
				delta = Input.mousePosition - previousMousePos;
				previousMousePos = Input.mousePosition;
			}

			Vector3 cameraDelta = swipeCamera.ScreenToViewportPoint(delta);
			cameraDelta.x *= -2f * swipeSpeedMultiplier;
			cameraDelta.y *= -2f * swipeSpeedMultiplier;
			cameraDelta.z = 0f;
			
			Vector3 cameraPos = swipeCamera.transform.position;
			
			cameraPos += cameraDelta;
			
			swipeCamera.transform.position = CalcCameraPosition(cameraPos, swipePanViewA, swipePanViewB);
			swipeCamera.orthographicSize = CalcCameraSize(cameraPos, swipePanViewA, swipePanViewB); 
		}
		
		// Clamp camera position to region defined by the two views
		protected virtual Vector3 CalcCameraPosition(Vector3 pos, View viewA, View viewB)
		{
			Vector3 safePos = pos;
			
			// Clamp camera position to region defined by the two views
			safePos.x = Mathf.Max(safePos.x, Mathf.Min(viewA.transform.position.x, viewB.transform.position.x));
			safePos.x = Mathf.Min(safePos.x, Mathf.Max(viewA.transform.position.x, viewB.transform.position.x));
			safePos.y = Mathf.Max(safePos.y, Mathf.Min(viewA.transform.position.y, viewB.transform.position.y));
			safePos.y = Mathf.Min(safePos.y, Mathf.Max(viewA.transform.position.y, viewB.transform.position.y));
			
			return safePos;
		}
		
		// Smoothly interpolate camera orthographic size based on relative position to two views
		protected virtual float CalcCameraSize(Vector3 pos, View viewA, View viewB)
		{
			// Get ray and point in same space
			Vector3 toViewB = viewB.transform.position - viewA.transform.position;
			Vector3 localPos = pos - viewA.transform.position;
			
			// Normalize
			float distance = toViewB.magnitude;
			toViewB /= distance;
			localPos /= distance;
			
			// Project point onto ray
			float t = Vector3.Dot(toViewB, localPos);
			t = Mathf.Clamp01(t); // Not really necessary but no harm
			
			float cameraSize = Mathf.Lerp(viewA.viewSize, viewB.viewSize, t);
			
			return cameraSize;
		}
	}
}