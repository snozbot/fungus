using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Load Scene", 
	             "Loads a new scene and displays an optional loading image. This is useful " +
	             "for splitting a large game across multiple scene files to reduce peak memory " +
	             "usage. All previously loaded assets (including textures and audio) will be released." +
	             "The scene to be loaded must be added to the scene list in Build Settings.")]
	public class LoadScene : Command 
	{
		public string sceneName = "";

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