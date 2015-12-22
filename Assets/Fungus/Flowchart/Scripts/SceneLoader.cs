using UnityEngine;
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#else
using UnityEngine.SceneManagement;
#endif
using System.Collections;
using System;

namespace Fungus
{
	/**
	 * Helper component for loading a new scene.
	 * A fullscreen loading image is displayed while loading the new scene.
	 * All Rooms are destroyed and unused assets are released from memory before loading the new scene to minimize memory footprint.
	 * For streaming Web Player builds, the loading image will be displayed until the requested level has finished downloading.
	 */
	public class SceneLoader : MonoBehaviour
	{
		protected Texture2D loadingTexture;
		protected string sceneToLoad;
		protected bool displayedImage;

		/**
		 * Asynchronously load a new scene.
		 * @param _sceneToLoad The name of the scene to load. Scenes must be added in project build settings.
		 * @param _loadingTexture Loading image to display while loading the new scene.
		 */
		static public void LoadScene(string _sceneToLoad, Texture2D _loadingTexture)
		{
			// Unity does not provide a way to check if the named scene actually exists in the project.
			GameObject go = new GameObject("SceneLoader");
			DontDestroyOnLoad(go);

			SceneLoader sceneLoader = go.AddComponent<SceneLoader>();
			sceneLoader.sceneToLoad = _sceneToLoad;
			sceneLoader.loadingTexture = _loadingTexture;
		}

		protected virtual void Start()
		{
			StartCoroutine(DoLoadBlock());
		}

		IEnumerator DoLoadBlock()
		{
			// Wait until loading image has been displayed in OnGUI
			while (loadingTexture != null && 
			       !displayedImage)
			{
				yield return new WaitForEndOfFrame();
			}

			// Sprites tend to take up most of the memory in a Fungus game, so destroy all sprite objects
			// first to free up memory for loading in the next scene.
			SpriteRenderer[] renderers = GameObject.FindObjectsOfType<SpriteRenderer>();
			foreach (SpriteRenderer renderer in renderers)
			{
				if (renderer != null)
				{
					DestroyImmediate(renderer.gameObject);
				}
			}

			// Wait for objects to actually be destroyed at end of run loop
			yield return new WaitForEndOfFrame();

			// All Room assets should no longer be referenced now, so unload them.
			yield return Resources.UnloadUnusedAssets();

			// Wait until scene has finished downloading (WebPlayer only)
			while (!Application.CanStreamedLevelBeLoaded(sceneToLoad))
			{
				yield return new WaitForEndOfFrame();
			}

			// Load the scene (happens at end of frame)
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			Application.LoadLevel(sceneToLoad);
#else
			SceneManager.LoadScene(sceneToLoad);
#endif

			yield return new WaitForEndOfFrame();

			// Clean up any remaining unused assets
			Resources.UnloadUnusedAssets();

			// We're now finished with the SceneLoader
			Destroy(gameObject);
		}

		protected virtual void OnGUI()
		{
			if (loadingTexture == null)
			{
				return;
			}

			GUI.depth = -2000;
			
			float h = Screen.height;
			float w = (float)loadingTexture.width * (h / (float)loadingTexture.height);
			
			float x = Screen.width / 2 - w / 2;
			float y = 0;
			
			Rect rect = new Rect(x, y, w, h);

			GUI.DrawTexture(rect, loadingTexture);

			if (Event.current.type == EventType.Repaint)
			{
				// Flag that image is now being shown
				displayedImage = true;
			}
		}
	}
}
