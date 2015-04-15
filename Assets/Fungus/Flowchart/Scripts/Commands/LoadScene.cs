using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Load Scene", 
	             "Loads a new Unity scene and displays an optional loading image. This is useful " +
	             "for splitting a large game across multiple scene files to reduce peak memory " +
	             "usage. Previously loaded assets will be released before loading the scene to free up memory." +
	             "The scene to be loaded must be added to the scene list in Build Settings.")]
	[AddComponentMenu("")]
	public class LoadScene : Command 
	{
		[Tooltip("Name of the scene to load. The scene must also be added to the build settings.")]
		public string sceneName = "";

		[Tooltip("Image to display while loading the scene")]
		public Texture2D loadingImage;

		public override void OnEnter()
		{
			SceneLoader.LoadScene(sceneName, loadingImage);
		}

		public override string GetSummary()
		{
			if (sceneName.Length == 0)
			{
				return "Error: No scene name selected";
			}

			return sceneName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}