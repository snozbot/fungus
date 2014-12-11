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
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Sprite/Prefabs/ClickableSprite.prefab");
		}

		[MenuItem("GameObject/Fungus/Sprite/DraggableSprite")]
		static void CreateDraggableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Sprite/Prefabs/DraggableSprite.prefab");
		}

		[MenuItem("GameObject/Fungus/Sprite/DragTargetSprite")]
		static void CreateDragTargetSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Sprite/Prefabs/DragTargetSprite.prefab");
		}

		[MenuItem("GameObject/Fungus/Sprite/ParallaxSprite")]
		static void CreateParallaxSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("Assets/Fungus/Sprite/Prefabs/ParallaxSprite.prefab");
		}
	}

}