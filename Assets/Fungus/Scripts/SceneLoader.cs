using UnityEngine;
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
		Texture2D loadingTexture;
		string sceneToLoad;
		bool displayedImage;
		bool saveCheckpoint;

		/**
		 * Asynchronously load a new scene.
		 * @param _sceneToLoad The name of the scene to load. Scenes must be added in project build settings.
		 * @param _loadingTexture Loading image to display while loading the new scene.
		 * @param _saveCheckpoint Automatically save a checkpoint once the new scene has loaded.
		 */
		static public void LoadScene(string _sceneToLoad, Texture2D _loadingTexture, bool _saveCheckpoint)
		{
			// Unity does not provide a way to check if the named scene actually exists in the project.
			GameObject go = new GameObject("SceneLoader");
			DontDestroyOnLoad(go);

			SceneLoader sceneLoader = go.AddComponent<SceneLoader>();
			sceneLoader.sceneToLoad = _sceneToLoad;
			sceneLoader.loadingTexture = _loadingTexture;
			sceneLoader.saveCheckpoint = _saveCheckpoint;
		}

		void Start()
		{
			StartCoroutine(DoLoadSequence());
		}

		IEnumerator DoLoadSequence()
		{
			// Wait until loading image has been displayed in OnGUI
			while (loadingTexture != null && 
			       !displayedImage)
			{
				yield return new WaitForEndOfFrame();
			}

			// Destroy all Room objects to release references to most game assets
			Room[] rooms = GameObject.FindObjectsOfType<Room>();
			foreach (Room room in rooms)
			{
				Destroy(room.gameObject);
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
			Application.LoadLevel(sceneToLoad);

			yield return new WaitForEndOfFrame();

			// Clean up any remaining unused assets
			Resources.UnloadUnusedAssets();

			// Save a checkpoint if required
			if (saveCheckpoint)
			{
				Game game = Game.GetInstance();
				if (game != null)
				{
					Game.Save();
				}
			}

			// We're now finished with the SceneLoader
			Destroy(gameObject);
		}

		void OnGUI()
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
