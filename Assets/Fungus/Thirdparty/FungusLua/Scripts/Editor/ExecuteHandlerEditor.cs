// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Adapted from the Unity Test Tools project (MIT license)
// https://bitbucket.org/Unity-Technologies/unitytesttools/src/a30d562427e9/Assets/UnityTestTools/

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fungus
{
    [CustomEditor(typeof(ExecuteHandler))]
    public class ExecuteHandlerEditor : Editor
    {
        private readonly DropDownControl<Type> m_ComparerDropDown = new DropDownControl<Type>();

        #region GUI Contents
        private readonly GUIContent m_GUIExecuteAfterTimeGuiContent = new GUIContent("Execute after (seconds)", "After how many seconds the script should be executed");
        private readonly GUIContent m_GUIRepeatExecuteTimeGuiContent = new GUIContent("Repeat execute", "Should the execution be repeated.");
        private readonly GUIContent m_GUIRepeatEveryTimeGuiContent = new GUIContent("Frequency of repetitions", "How often should the execution be done");
        private readonly GUIContent m_GUIExecuteAfterFramesGuiContent = new GUIContent("Execute after (frames)", "After how many frames the script should be executed");
        private readonly GUIContent m_GUIRepeatExecuteFrameGuiContent = new GUIContent("Repeat execution", "Should the execution be repeated.");
        #endregion

        public ExecuteHandlerEditor()
        {
            m_ComparerDropDown.convertForButtonLabel = type => type.Name;
            m_ComparerDropDown.convertForGUIContent = type => type.Name;
            m_ComparerDropDown.ignoreConvertForGUIContent = types => false;
            m_ComparerDropDown.tooltip = "Comparer that will be used to compare values and determine the result of assertion.";
        }

        public override void OnInspectorGUI()
        {
            var executeHandler = (ExecuteHandler)target;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("On Event"));
#if UNITY_2017_3_OR_NEWER
            executeHandler.ExecuteMethods = (ExecuteMethod)EditorGUILayout.EnumFlagsField(executeHandler.ExecuteMethods,
                                                                                         EditorStyles.popup,
                                                                                         GUILayout.ExpandWidth(false));
#else
            executeHandler.ExecuteMethods = (ExecuteMethod)EditorGUILayout.EnumMaskField(executeHandler.ExecuteMethods,
                                                                                         EditorStyles.popup,
                                                                                         GUILayout.ExpandWidth(false));
#endif

            EditorGUILayout.EndHorizontal();

            if (executeHandler.IsExecuteMethodSelected(ExecuteMethod.AfterPeriodOfTime))
            {
                DrawOptionsForAfterPeriodOfTime(executeHandler);
            }

            if (executeHandler.IsExecuteMethodSelected(ExecuteMethod.Update))
            {
                DrawOptionsForOnUpdate(executeHandler);
            }

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button(new GUIContent("Execute Now", "Execute the script immediately.")))
                {
                    executeHandler.Execute();
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawOptionsForAfterPeriodOfTime(ExecuteHandler executeHandler)
        {
            EditorGUILayout.Space();
            executeHandler.ExecuteAfterTime = EditorGUILayout.FloatField(m_GUIExecuteAfterTimeGuiContent,
                                                               executeHandler.ExecuteAfterTime);
            if (executeHandler.ExecuteAfterTime < 0)
                executeHandler.ExecuteAfterTime = 0;
            executeHandler.RepeatExecuteTime = EditorGUILayout.Toggle(m_GUIRepeatExecuteTimeGuiContent,
                                                            executeHandler.RepeatExecuteTime);
            if (executeHandler.RepeatExecuteTime)
            {
                executeHandler.RepeatEveryTime = EditorGUILayout.FloatField(m_GUIRepeatEveryTimeGuiContent,
                                                                    executeHandler.RepeatEveryTime);
                if (executeHandler.RepeatEveryTime < 0)
                    executeHandler.RepeatEveryTime = 0;
            }
        }

        private void DrawOptionsForOnUpdate(ExecuteHandler executeHandler)
        {
            EditorGUILayout.Space();
            executeHandler.ExecuteAfterFrames = EditorGUILayout.IntField(m_GUIExecuteAfterFramesGuiContent,
                                                               executeHandler.ExecuteAfterFrames);
            if (executeHandler.ExecuteAfterFrames < 1)
                executeHandler.ExecuteAfterFrames = 1;
            executeHandler.RepeatExecuteFrame = EditorGUILayout.Toggle(m_GUIRepeatExecuteFrameGuiContent,
                                                             executeHandler.RepeatExecuteFrame);
            if (executeHandler.RepeatExecuteFrame)
            {
                executeHandler.RepeatEveryFrame = EditorGUILayout.IntField(m_GUIRepeatEveryTimeGuiContent,
                                                                   executeHandler.RepeatEveryFrame);
                if (executeHandler.RepeatEveryFrame < 1)
                    executeHandler.RepeatEveryFrame = 1;
            }
        }
    }
}
