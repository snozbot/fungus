using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/**
	 * Prevents saving of selected sequence and commands to avoid version control conflicts
	 */
	public class AssetModProcessor : UnityEditor.AssetModificationProcessor
	{
		public static string[] OnWillSaveAssets(string[] paths)
		{
			string sceneName = "";
			
			foreach (string path in paths)
			{
				if (path.Contains(".unity"))
				{
					sceneName = Path.GetFileNameWithoutExtension(path);
				}
			}
			
			if (sceneName.Length == 0)
			{
				return paths;
			}

			// Reset these variables before save so that they won't cause a git conflict
			FungusScript[] allFungusScripts = UnityEngine.Object.FindObjectsOfType<FungusScript>();
			foreach (FungusScript fs in allFungusScripts)
			{
				if (!fs.saveSelection)
				{
					fs.selectedSequence = null;
					fs.ClearSelectedCommands();
				}
			}

			return paths;
		}

	}
}