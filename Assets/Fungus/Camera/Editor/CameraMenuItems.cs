using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class CameraMenuItems 
	{
		[MenuItem("GameObject/Fungus/Camera/View")]
		static void CreateBackground()
		{
			InstantiatePrefab("View");
		}

		static void InstantiatePrefab(string prefabName)
		{
			GameObject prefab = Resources.LoadAssetAtPath("Assets/Fungus/Camera/Prefabs/" + prefabName + ".prefab", typeof(GameObject)) as GameObject;
			if (prefab != null)
			{
				GameObject go = GameObject.Instantiate(prefab) as GameObject;
				go.name = prefabName;
			}
		}
	}

}