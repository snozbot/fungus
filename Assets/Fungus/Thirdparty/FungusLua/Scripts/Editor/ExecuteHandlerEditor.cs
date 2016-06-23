/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

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
            executeHandler.executeMethods = (ExecuteHandler.ExecuteMethod)EditorGUILayout.EnumMaskField(executeHandler.executeMethods,
                                                                                                    EditorStyles.popup,
                                                                                                    GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (executeHandler.IsExecuteMethodSelected(ExecuteHandler.ExecuteMethod.AfterPeriodOfTime))
            {
                DrawOptionsForAfterPeriodOfTime(executeHandler);
            }

            if (executeHandler.IsExecuteMethodSelected(ExecuteHandler.ExecuteMethod.Update))
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
            executeHandler.executeAfterTime = EditorGUILayout.FloatField(m_GUIExecuteAfterTimeGuiContent,
                                                               executeHandler.executeAfterTime);
            if (executeHandler.executeAfterTime < 0)
                executeHandler.executeAfterTime = 0;
            executeHandler.repeatExecuteTime = EditorGUILayout.Toggle(m_GUIRepeatExecuteTimeGuiContent,
                                                            executeHandler.repeatExecuteTime);
            if (executeHandler.repeatExecuteTime)
            {
                executeHandler.repeatEveryTime = EditorGUILayout.FloatField(m_GUIRepeatEveryTimeGuiContent,
                                                                    executeHandler.repeatEveryTime);
                if (executeHandler.repeatEveryTime < 0)
                    executeHandler.repeatEveryTime = 0;
            }
        }

        private void DrawOptionsForOnUpdate(ExecuteHandler executeHandler)
        {
            EditorGUILayout.Space();
            executeHandler.executeAfterFrames = EditorGUILayout.IntField(m_GUIExecuteAfterFramesGuiContent,
                                                               executeHandler.executeAfterFrames);
            if (executeHandler.executeAfterFrames < 1)
                executeHandler.executeAfterFrames = 1;
            executeHandler.repeatExecuteFrame = EditorGUILayout.Toggle(m_GUIRepeatExecuteFrameGuiContent,
                                                             executeHandler.repeatExecuteFrame);
            if (executeHandler.repeatExecuteFrame)
            {
                executeHandler.repeatEveryFrame = EditorGUILayout.IntField(m_GUIRepeatEveryTimeGuiContent,
                                                                   executeHandler.repeatEveryFrame);
                if (executeHandler.repeatEveryFrame < 1)
                    executeHandler.repeatEveryFrame = 1;
            }
        }
    }
}
