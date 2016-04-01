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
        private readonly DropDownControl<Type> m_ComparerDropDown = new DropDownControl<Type>();

        #region GUI Contents
        private readonly GUIContent m_GUIExecuteAfterTimeGuiContent = new GUIContent("Execute after (seconds)", "After how many seconds the script should be executed");
        private readonly GUIContent m_GUIRepeatExecuteTimeGuiContent = new GUIContent("Repeat execute", "Should the execution be repeated.");
        private readonly GUIContent m_GUIRepeatEveryTimeGuiContent = new GUIContent("Frequency of repetitions", "How often should the execution be done");
        private readonly GUIContent m_GUIExecuteAfterFramesGuiContent = new GUIContent("Execute after (frames)", "After how many frames the script should be executed");
        private readonly GUIContent m_GUIRepeatExecuteFrameGuiContent = new GUIContent("Repeat execution", "Should the execution be repeated.");
        #endregion

        protected SerializedProperty luaEnvironmentProp;
        protected SerializedProperty runAsCoroutineProp;
        protected SerializedProperty luaFileProp;
        protected SerializedProperty luaScriptProp;
        protected SerializedProperty useFungusModuleProp;

        protected List<TextAsset> luaFiles = new List<TextAsset>();

		public LuaScriptEditor()
        {
            m_ComparerDropDown.convertForButtonLabel = type => type.Name;
            m_ComparerDropDown.convertForGUIContent = type => type.Name;
            m_ComparerDropDown.ignoreConvertForGUIContent = types => false;
            m_ComparerDropDown.tooltip = "Comparer that will be used to compare values and determine the result of assertion.";
        }

        public virtual void OnEnable()
        {
            luaEnvironmentProp = serializedObject.FindProperty("luaEnvironment");
            runAsCoroutineProp = serializedObject.FindProperty("runAsCoroutine");
            luaFileProp = serializedObject.FindProperty("luaFile");
            luaScriptProp = serializedObject.FindProperty("luaScript");
            useFungusModuleProp = serializedObject.FindProperty("useFungusModule");

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
            var fungusInvoke = (LuaScript)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("On Event"));
            fungusInvoke.executeMethods = (LuaScript.ExecuteMethod)EditorGUILayout.EnumMaskField(fungusInvoke.executeMethods,
                                                                                                    EditorStyles.popup,
                                                                                                    GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (fungusInvoke.IsExecuteMethodSelected(LuaScript.ExecuteMethod.AfterPeriodOfTime))
            {
                DrawOptionsForAfterPeriodOfTime(fungusInvoke);
            }

            if (fungusInvoke.IsExecuteMethodSelected(LuaScript.ExecuteMethod.Update))
            {
                DrawOptionsForOnUpdate(fungusInvoke);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Execute Now", "Execute the Lua script immediately.")))
                {
                    fungusInvoke.Execute();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

            serializedObject.Update();

            EditorGUILayout.PropertyField(luaEnvironmentProp);
            EditorGUILayout.PropertyField(runAsCoroutineProp);
            EditorGUILayout.PropertyField(useFungusModuleProp);

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

        private void DrawOptionsForAfterPeriodOfTime(LuaScript script)
        {
            EditorGUILayout.Space();
            script.executeAfterTime = EditorGUILayout.FloatField(m_GUIExecuteAfterTimeGuiContent,
                                                               script.executeAfterTime);
            if (script.executeAfterTime < 0)
                script.executeAfterTime = 0;
            script.repeatExecuteTime = EditorGUILayout.Toggle(m_GUIRepeatExecuteTimeGuiContent,
                                                            script.repeatExecuteTime);
            if (script.repeatExecuteTime)
            {
                script.repeatEveryTime = EditorGUILayout.FloatField(m_GUIRepeatEveryTimeGuiContent,
                                                                    script.repeatEveryTime);
                if (script.repeatEveryTime < 0)
                    script.repeatEveryTime = 0;
            }
        }

        private void DrawOptionsForOnUpdate(LuaScript script)
        {
            EditorGUILayout.Space();
            script.executeAfterFrames = EditorGUILayout.IntField(m_GUIExecuteAfterFramesGuiContent,
                                                               script.executeAfterFrames);
            if (script.executeAfterFrames < 1)
                script.executeAfterFrames = 1;
            script.repeatExecuteFrame = EditorGUILayout.Toggle(m_GUIRepeatExecuteFrameGuiContent,
                                                             script.repeatExecuteFrame);
            if (script.repeatExecuteFrame)
            {
                script.repeatEveryFrame = EditorGUILayout.IntField(m_GUIRepeatEveryTimeGuiContent,
                                                                   script.repeatEveryFrame);
                if (script.repeatEveryFrame < 1)
                    script.repeatEveryFrame = 1;
            }
        }
    }
}
