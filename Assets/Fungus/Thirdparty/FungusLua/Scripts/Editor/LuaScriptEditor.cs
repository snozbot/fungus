/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    
    [CustomEditor (typeof(LuaScript))]
    public class LuaScriptEditor : Editor
    {
        protected SerializedProperty luaEnvironmentProp;
        protected SerializedProperty luaFileProp;
        protected SerializedProperty luaScriptProp;
        protected SerializedProperty runAsCoroutineProp;

        protected virtual void OnEnable()
        {
            luaEnvironmentProp = serializedObject.FindProperty("luaEnvironment");
            luaFileProp = serializedObject.FindProperty("luaFile");
            luaScriptProp = serializedObject.FindProperty("luaScript");
            runAsCoroutineProp = serializedObject.FindProperty("runAsCoroutine");
        }
        
        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(luaEnvironmentProp);
            EditorGUILayout.PropertyField(luaFileProp);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(luaScriptProp);

            if (EditorGUI.EndChangeCheck() &&
                EditorApplication.isPlaying)
            {
                // Reinitialise if the Lua script is changed while running in editor
                LuaScript luaScript = target as LuaScript;
                luaScript.initialised = false;
            }

            EditorGUILayout.PropertyField(runAsCoroutineProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
    
}