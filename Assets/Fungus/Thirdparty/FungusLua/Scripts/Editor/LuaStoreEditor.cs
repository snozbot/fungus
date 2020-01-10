// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEditor;
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
            if (luaStore.PrimeTable != null)
            {
                EditorGUILayout.PrefixLabel(new GUIContent("Inspect Table", "Displays the contents of the fungus.store prime table."));
                string serialized = luaStore.PrimeTable.Serialize();
                EditorGUILayout.SelectableLabel(serialized, GUILayout.ExpandHeight(true));
            }
        }
    }
}
