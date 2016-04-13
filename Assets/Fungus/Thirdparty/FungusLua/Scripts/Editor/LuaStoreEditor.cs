using UnityEngine;
using UnityEditor;
using System.Collections;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Serialization;

namespace Fungus
{

	[CustomEditor(typeof(LuaStore))]
	public class LuaStoreEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			// Use the Serialization extension to display the contents of the prime table.
			LuaStore luaStore = target as LuaStore;
			if (luaStore.primeTable != null)
			{
				EditorGUILayout.PrefixLabel(new GUIContent("Inspect Table", "Displays the contents of the fungus.store prime table."));
				string serialized = luaStore.primeTable.Serialize();
				EditorGUILayout.SelectableLabel(serialized, GUILayout.ExpandHeight(true));
			}
		}
	}

}
