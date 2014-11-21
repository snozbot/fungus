using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class SpriteMenuItems 
	{
		[MenuItem("GameObject/Fungus/Sprite/ClickableSprite")]
		static void CreateClickableSprite()
		{
			InstantiatePrefab("ClickableSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/DraggableSprite")]
		static void CreateDraggableSprite()
		{
			InstantiatePrefab("DraggableSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/DragTargetSprite")]
		static void CreateDragTargetSprite()
		{
			InstantiatePrefab("DragTargetSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/ParallaxSprite")]
		static void CreateParallaxSprite()
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
				Undo.RegisterCreatedObjectUndo(go, go.name);
				Selection.activeGameObject = go;
			}
		}

	}

}