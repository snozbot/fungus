using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class SpriteMenuItems 
	{

		[MenuItem("GameObject/Fungus/Sprite/ParallaxSprite")]
		static void CreateBackground()
		{
			InstantiatePrefab("ParallaxSprite");
		}

		static void InstantiatePrefab(string prefabName)
		{
			GameObject prefab = Resources.LoadAssetAtPath("Assets/Fungus/Sprite/Prefabs/" + prefabName + ".prefab", typeof(GameObject)) as GameObject;
			if (prefab != null)
			{
				GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
				go.name = prefabName;
			}
		}

	}

}