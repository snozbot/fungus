// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    public class VariableListAdaptor
    {

        protected class AddVariableInfo
        {
            public Flowchart flowchart;
            public System.Type variableType;
        }

        public static readonly int DefaultWidth = 80 + 100 + 140 + 60;
        public static readonly int ScrollSpacer = 0;
        public static readonly int ReorderListSkirts = 50;

        protected SerializedProperty _arrayProperty;

        public float fixedItemHeight;
        public int widthOfList;

        private ReorderableList list;
        private Flowchart targetFlowchart;
        
        public SerializedProperty this[int index]
        {
            get { return _arrayProperty.GetArrayElementAtIndex(index); }
        }

        public SerializedProperty arrayProperty
        {
            get { return _arrayProperty; }
        }

        public VariableListAdaptor(SerializedProperty arrayProperty, float fixedItemHeight, int widthOfList, Flowchart _targetFlowchart)
        {
            if (arrayProperty == null)
                throw new ArgumentNullException("Array property was null.");
            if (!arrayProperty.isArray)
                throw new InvalidOperationException("Specified serialized propery is not an array.");

            this.targetFlowchart = _targetFlowchart;
            this._arrayProperty = arrayProperty;
            this.fixedItemHeight = fixedItemHeight;
            this.widthOfList = widthOfList - ScrollSpacer;
            list = new ReorderableList(arrayProperty.serializedObject, arrayProperty, true, false, true, true);
            list.drawElementCallback = DrawItem;
            list.onRemoveCallback = RemoveItem;
            //list.drawHeaderCallback = DrawHeader;
            list.onAddCallback = AddButton;
            list.onRemoveCallback = RemoveItem;
        }

        private void RemoveItem(ReorderableList list)
        {
            int index = list.index;
            // Remove the Fungus Variable component
            Variable variable = _arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue as Variable;
            Undo.DestroyObjectImmediate(variable);
        }

        private void AddButton(ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();
            List<System.Type> types = FlowchartEditor.FindAllDerivedTypes<Variable>();

            // Add variable types without a category
            foreach (var type in types)
            {
                VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
                if (variableInfo == null ||
                    variableInfo.Category != "")
                {
                    continue;
                }

                AddVariableInfo addVariableInfo = new AddVariableInfo();
                addVariableInfo.flowchart = targetFlowchart;
                addVariableInfo.variableType = type;

                GUIContent typeName = new GUIContent(variableInfo.VariableType);

                menu.AddItem(typeName, false, AddVariable, addVariableInfo);
            }

            // Add types with a category
            foreach (var type in types)
            {
                VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
                if (variableInfo == null ||
                    variableInfo.Category == "")
                {
                    continue;
                }

                AddVariableInfo info = new AddVariableInfo();
                info.flowchart = targetFlowchart;
                info.variableType = type;

                GUIContent typeName = new GUIContent(variableInfo.Category + "/" + variableInfo.VariableType);

                menu.AddItem(typeName, false, AddVariable, info);
            }

            menu.ShowAsContext();
        }

        protected virtual void AddVariable(object obj)
        {
            AddVariableInfo addVariableInfo = obj as AddVariableInfo;
            if (addVariableInfo == null)
            {
                return;
            }

            var flowchart = addVariableInfo.flowchart;
            System.Type variableType = addVariableInfo.variableType;

            Undo.RecordObject(flowchart, "Add Variable");
            Variable newVariable = flowchart.gameObject.AddComponent(variableType) as Variable;
            newVariable.Key = flowchart.GetUniqueVariableKey("");
            flowchart.Variables.Add(newVariable);

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.PrefixLabel(rect, new GUIContent("Variables"));
        }
        
        public void DrawVarList(int w)
        {
            this.widthOfList = (w == 0 ? VariableListAdaptor.DefaultWidth : w) - ScrollSpacer;

            if(GUILayout.Button("Variables"))
            {
                arrayProperty.isExpanded = !arrayProperty.isExpanded;
            }

            if (arrayProperty.isExpanded)
            {
                list.DoLayoutList();
            }
        }

        public void DrawItem(Rect position, int index, bool selected, bool focused)
        {
            Variable variable = this[index].objectReferenceValue as Variable;

            if (variable == null)
            {
                return;
            }

            int width = widthOfList;
            int totalRatio = DefaultWidth;


            float[] widths = { (80.0f/ totalRatio) * width,
                (100.0f / totalRatio) * width,
                (140.0f/ totalRatio) * width,
                (60.0f/ totalRatio) * width };
            Rect[] rects = new Rect[4];

            for (int i = 0; i < 4; ++i)
            {
                rects[i] = position;
                rects[i].width = widths[i] - 5;

                for (int j = 0; j < i; ++j)
                {
                    rects[i].x += widths[j];
                }
            }

            VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(variable.GetType());
            if (variableInfo == null)
            {
                return;
            }

            var flowchart = targetFlowchart;
            if (flowchart == null)
            {
                return;
            }

            // Highlight if an active or selected command is referencing this variable
            bool highlight = false;
            if (flowchart.SelectedBlock != null)
            {
                if (Application.isPlaying && flowchart.SelectedBlock.IsExecuting())
                {
                    highlight = flowchart.SelectedBlock.ActiveCommand.HasReference(variable);
                }
                else if (!Application.isPlaying && flowchart.SelectedCommands.Count > 0)
                {
                    foreach (Command selectedCommand in flowchart.SelectedCommands)
                    {
                        if (selectedCommand == null)
                        {
                            continue;
                        }

                        if (selectedCommand.HasReference(variable))
                        {
                            highlight = true;
                            break;
                        }
                    }
                }
            }

            if (highlight)
            {
                GUI.backgroundColor = Color.green;
                GUI.Box(position, "");
            }

            string key = variable.Key;
            VariableScope scope = variable.Scope;

            // To access properties in a monobehavior, you have to new a SerializedObject
            // http://answers.unity3d.com/questions/629803/findrelativeproperty-never-worked-for-me-how-does.html
            SerializedObject variableObject = new SerializedObject(this[index].objectReferenceValue);

            variableObject.Update();

            GUI.Label(rects[0], variableInfo.VariableType);

            key = EditorGUI.TextField(rects[1], variable.Key);
            SerializedProperty keyProp = variableObject.FindProperty("key");
            SerializedProperty defaultProp = variableObject.FindProperty("value");
            SerializedProperty scopeProp = variableObject.FindProperty("scope");

            keyProp.stringValue = flowchart.GetUniqueVariableKey(key, variable);

            bool isGlobal = scopeProp.enumValueIndex == (int)VariableScope.Global;


            if (isGlobal && Application.isPlaying)
            {
                var res = FungusManager.Instance.GlobalVariables.GetVariable(keyProp.stringValue);
                if (res != null)
                {
                    SerializedObject globalValue = new SerializedObject(res);
                    var globalValProp = globalValue.FindProperty("value");

                    var prevEnabled = GUI.enabled;
                    GUI.enabled = false;

                    EditorGUI.PropertyField(rects[2], globalValProp, new GUIContent(""));

                    GUI.enabled = prevEnabled;
                }
            }
            else
            {
                EditorGUI.PropertyField(rects[2], defaultProp, new GUIContent(""));
            }


            scope = (VariableScope)EditorGUI.EnumPopup(rects[3], variable.Scope);
            scopeProp.enumValueIndex = (int)scope;

            variableObject.ApplyModifiedProperties();

            GUI.backgroundColor = Color.white;
        }
    }
}

