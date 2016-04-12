// Adapted from the Unity Test Tools project (MIT license)
// https://bitbucket.org/Unity-Technologies/unitytesttools/src/a30d562427e9/Assets/UnityTestTools/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fungus
{
    
    [CustomEditor(typeof(LuaScript))]
    public class LuaScriptEditor : Editor
    {
        protected SerializedProperty luaEnvironmentProp;
        protected SerializedProperty runAsCoroutineProp;
        protected SerializedProperty luaFileProp;
        protected SerializedProperty luaScriptProp;

        protected List<TextAsset> luaFiles = new List<TextAsset>();

        public virtual void OnEnable()
        {
            luaEnvironmentProp = serializedObject.FindProperty("luaEnvironment");
            runAsCoroutineProp = serializedObject.FindProperty("runAsCoroutine");
            luaFileProp = serializedObject.FindProperty("luaFile");
            luaScriptProp = serializedObject.FindProperty("luaScript");

            // Make a note of all Lua files in the project resources
            object[] result = Resources.LoadAll("Lua", typeof(TextAsset));
            luaFiles.Clear();
            foreach (object res in result)
            {
                TextAsset ta = res as TextAsset;

                // Ignore the built-in modules as you'll never want to execute these
                if (ta == null ||
                    ta.name == "fungus" ||
                    ta.name == "inspect")
                {
                    continue;
                }

                luaFiles.Add(ta);
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(luaEnvironmentProp);
            EditorGUILayout.PropertyField(runAsCoroutineProp);

            int selected = 0;
            List<GUIContent> options = new List<GUIContent>();
            options.Add(new GUIContent("<Lua Script>"));
            int index = 1;
            if (luaFiles != null)
            {
                foreach (TextAsset textAsset in luaFiles)
                {
                    options.Add(new GUIContent(textAsset.name));

                    if (luaFileProp.objectReferenceValue == textAsset)
                    {
                        selected = index;
                    }

                    index++;
                }
            }

            selected = EditorGUILayout.Popup(new GUIContent("Execute Lua", "Lua file or script to execute."), selected, options.ToArray());
            if (selected == 0)
            {
                luaFileProp.objectReferenceValue = null;
            }
            else
            {                
                luaFileProp.objectReferenceValue = luaFiles[selected - 1];
            }

            if (luaFileProp.objectReferenceValue == null)
            {
                EditorGUILayout.PropertyField(luaScriptProp);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Open in Editor", "Open this Lua file in the external text editor.")))
                {
                    string path = AssetDatabase.GetAssetPath(luaFileProp.objectReferenceValue);
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, 0);
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
