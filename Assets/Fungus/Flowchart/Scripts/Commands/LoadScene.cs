using UnityEngine;
using UnityEngine.Serialization;
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
	public class LoadScene : Command, ISerializationCallbackReceiver 
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("sceneName")] public string sceneNameOLD;
		#endregion

		[Tooltip("Name of the scene to load. The scene must also be added to the build settings.")]
		public StringData _sceneName = new StringData("");

		[Tooltip("Image to display while loading the scene")]
		public Texture2D loadingImage;

		public override void OnEnter()
		{
			SceneLoader.LoadScene(_sceneName.Value, loadingImage);
		}

		public override string GetSummary()
		{
			if (_sceneName.Value.Length == 0)
			{
				return "Error: No scene name selected";
			}

			return _sceneName;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (sceneNameOLD != default(string))
			{
				_sceneName.Value = sceneNameOLD;
				sceneNameOLD = default(string);
			}
		}
	}

}