using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class DialogMenuItems 
	{

		[MenuItem("GameObject/Fungus/Dialog/Background")]
		static void CreateBackground()
		{
			InstantiatePrefab("Background");
		}

		[MenuItem("GameObject/Fungus/Dialog/Character")]
		static void CreateCharacter()
		{
			InstantiatePrefab("Character");
		}

		[MenuItem("GameObject/Fungus/Dialog/NarratorDialog")]
		static void CreateNarratorDialog()
		{
			InstantiatePrefab("NarratorDialog");
		}

		[MenuItem("GameObject/Fungus/Dialog/SayDialog")]
		static void CreateSayDialog()
		{
			InstantiatePrefab("SayDialog");
		}

		[MenuItem("GameObject/Fungus/Dialog/ChooseDialog")]
		static void CreateChooseDialog()
		{
			InstantiatePrefab("ChooseDialog");
		}

		static void InstantiatePrefab(string prefabName)
		{
			GameObject prefab = Resources.LoadAssetAtPath("Assets/Fungus/Dialog/Prefabs/" + prefabName + ".prefab", typeof(GameObject)) as GameObject;
			if (prefab != null)
			{
				GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				go.name = prefabName;
			}
		}

	}

}