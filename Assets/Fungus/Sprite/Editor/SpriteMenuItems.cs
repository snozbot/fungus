using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{

	public class SpriteMenuItems 
	{
		[MenuItem("Tools/Fungus/Create/Clickable Sprite", false, 150)]
		static void CreateClickableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("ClickableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Draggable Sprite", false, 151)]
		static void CreateDraggableSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DraggableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Drag Target Sprite", false, 152)]
		static void CreateDragTargetSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("DragTargetSprite");
		}

		[MenuItem("Tools/Fungus/Create/Parallax Sprite", false, 152)]
		static void CreateParallaxSprite()
		{
			FungusScriptMenuItems.SpawnPrefab("ParallaxSprite");
		}
	}

}