using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CustomEditor (typeof(Game))]
	public class GameEditor : Editor
	{
		private void OnSceneGUI()
		{
			GameEditor.DrawRoomNames();
		}

		static public void DrawRoomNames()
		{
			Handles.color = Color.white;
			Handles.BeginGUI();
			
			// Show labels for each room
			Room[] rooms = GameObject.FindObjectsOfType<Room>();
			
			foreach (Room room in rooms)
			{
				if (!room.renderer)
				{
					continue;
				}

				Bounds bounds = room.renderer.bounds;
				Vector3 pos = new Vector3(bounds.min.x, bounds.max.y, 0);
				
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,1,1);
				style.fontSize /= 2;
				Rect boxRect = HandleUtility.WorldPointToSizedRect(pos, new GUIContent(room.name), style);
				boxRect.y -= boxRect.height * 1.5f;
				GUI.Box(boxRect, room.name, style);
			}
			
			Handles.EndGUI();
		}
	}
}