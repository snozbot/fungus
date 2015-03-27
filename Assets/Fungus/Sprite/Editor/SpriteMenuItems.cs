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
			FungusScriptMenuItems.SpawnPrefab("ClickableSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/DraggableSprite")]
		static void CreateDraggableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DraggableSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/DragTargetSprite")]
		static void CreateDragTargetSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DragTargetSprite");
		}

		[MenuItem("GameObject/Fungus/Sprite/ParallaxSprite")]
		static void CreateParallaxSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("ParallaxSprite");
		}
	}

}