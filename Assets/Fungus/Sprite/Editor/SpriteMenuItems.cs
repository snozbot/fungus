using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class SpriteMenuItems 
	{
		[MenuItem("Tools/Fungus/Create/Clickable Sprite")]
		static void CreateClickableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("ClickableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Draggable Sprite")]
		static void CreateDraggableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DraggableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Drag Target Sprite")]
		static void CreateDragTargetSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DragTargetSprite");
		}

		[MenuItem("Tools/Fungus/Create/Parallax Sprite")]
		static void CreateParallaxSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("ParallaxSprite");
		}
	}

}