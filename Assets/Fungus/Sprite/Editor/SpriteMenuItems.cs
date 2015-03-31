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
			FlowchartMenuItems.SpawnPrefab("ClickableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Draggable Sprite", false, 151)]
		static void CreateDraggableSprite()
		{
			FlowchartMenuItems.SpawnPrefab("DraggableSprite");
		}

		[MenuItem("Tools/Fungus/Create/Drag Target Sprite", false, 152)]
		static void CreateDragTargetSprite()
		{
			FlowchartMenuItems.SpawnPrefab("DragTargetSprite");
		}

		[MenuItem("Tools/Fungus/Create/Parallax Sprite", false, 152)]
		static void CreateParallaxSprite()
		{
			FlowchartMenuItems.SpawnPrefab("ParallaxSprite");
		}
	}

}