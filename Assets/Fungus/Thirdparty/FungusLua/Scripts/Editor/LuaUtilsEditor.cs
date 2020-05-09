// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Fungus
{   
    [CustomEditor (typeof(LuaUtils))]
    public class LuaUtilsEditor : Editor 
    {
        protected SerializedProperty stringTablesProp;
        protected ReorderableList stringTablesList;

        protected SerializedProperty registerTypesProp;
        protected ReorderableList registerTypeList;

        protected virtual void OnEnable()
        {
            // String Tables property
            stringTablesProp = serializedObject.FindProperty("stringTables");
            stringTablesList = new ReorderableList(serializedObject, stringTablesProp, true, true, true, true);

            stringTablesList.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, "String Tables");
            };

            stringTablesList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => { 
                Rect r = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                SerializedProperty element = stringTablesProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(r, element, new GUIContent(""));
            };

            // Register Types property   
            registerTypesProp = serializedObject.FindProperty("registerTypes");
            registerTypeList = new ReorderableList(serializedObject, registerTypesProp, true, true, true, true);

            registerTypeList.drawHeaderCallback = (Rect rect) => {  
                EditorGUI.LabelField(rect, "Type Lists");
            };

            registerTypeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => { 
                Rect r = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
                SerializedProperty element = registerTypesProp.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(r, element, new GUIContent(""));
            };
        }

        public override void OnInspectorGUI() 
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            EditorGUILayout.PrefixLabel(new GUIContent("String Tables", "A list of JSON files containing localised strings. These strings are loaded into a 'stringtable' global variable."));
            stringTablesList.DoLayoutList();

            EditorGUILayout.PrefixLabel(new GUIContent("Register Types", "Text files which list the CLR types that should be registered with this Lua environment."));
            registerTypeList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
   }
}
