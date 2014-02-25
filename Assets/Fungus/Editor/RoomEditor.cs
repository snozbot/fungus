using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CustomEditor (typeof(Room), true)]
	[CanEditMultipleObjects]
	public class RoomEditor : Editor
	{
		private void OnSceneGUI()
		{
			GameEditor.DrawRoomNames();
		}
	}
}