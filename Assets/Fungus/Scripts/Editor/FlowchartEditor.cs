// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;
using System.Reflection;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Flowchart))]
    public class FlowchartEditor : Editor 
    {
        protected class AddVariableInfo
        {
            public Flowchart flowchart;
            public System.Type variableType;
        }

        protected SerializedProperty descriptionProp;
        protected SerializedProperty colorCommandsProp;
        protected SerializedProperty hideComponentsProp;
        protected SerializedProperty stepPauseProp;
        protected SerializedProperty saveSelectionProp;
        protected SerializedProperty localizationIdProp;
        protected SerializedProperty variablesProp;
        protected SerializedProperty showLineNumbersProp;
        protected SerializedProperty hideCommandsProp;
        protected SerializedProperty luaEnvironmentProp;
        protected SerializedProperty luaBindingNameProp;

        protected Texture2D addTexture;
                
        protected virtual void OnEnable()
        {
            if (NullTargetCheck()) // Check for an orphaned editor instance
                return;

            descriptionProp = serializedObject.FindProperty("description");
            colorCommandsProp = serializedObject.FindProperty("colorCommands");
            hideComponentsProp = serializedObject.FindProperty("hideComponents");
            stepPauseProp = serializedObject.FindProperty("stepPause");
            saveSelectionProp = serializedObject.FindProperty("saveSelection");
            localizationIdProp = serializedObject.FindProperty("localizationId");
            variablesProp = serializedObject.FindProperty("variables");
            showLineNumbersProp = serializedObject.FindProperty("showLineNumbers");
            hideCommandsProp = serializedObject.FindProperty("hideCommands");
            luaEnvironmentProp = serializedObject.FindProperty("luaEnvironment");
            luaBindingNameProp = serializedObject.FindProperty("luaBindingName");

            addTexture = FungusEditorResources.AddSmall;
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            var flowchart = target as Flowchart;

            flowchart.UpdateHideFlags();

            EditorGUILayout.PropertyField(descriptionProp);
            EditorGUILayout.PropertyField(colorCommandsProp);
            EditorGUILayout.PropertyField(hideComponentsProp);
            EditorGUILayout.PropertyField(stepPauseProp);
            EditorGUILayout.PropertyField(saveSelectionProp);
            EditorGUILayout.PropertyField(localizationIdProp);
            EditorGUILayout.PropertyField(showLineNumbersProp);
            EditorGUILayout.PropertyField(luaEnvironmentProp);
            EditorGUILayout.PropertyField(luaBindingNameProp);

            // Show list of commands to hide in Add Command menu
            ReorderableListGUI.Title(new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip));
            ReorderableListGUI.ListField(hideCommandsProp);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Open Flowchart Window", "Opens the Flowchart Window")))
            {
                EditorWindow.GetWindow(typeof(FlowchartWindow), false, "Flowchart");
            }


            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            serializedObject.ApplyModifiedProperties();

            //Show the variables in the flowchart inspector
            GUILayout.Space(20);

            DrawVariablesGUI(false, Mathf.FloorToInt(EditorGUIUtility.currentViewWidth) - VariableListAdaptor.ReorderListSkirts);

        }

        public virtual void DrawVariablesGUI(bool showVariableToggleButton, int w)
        {
            serializedObject.Update();

            var t = target as Flowchart;

            if (t.Variables.Count == 0)
            {
                t.VariablesExpanded = true;
                //showVariableToggleButton = true;
            }

            if (showVariableToggleButton && !t.VariablesExpanded)
            {
                if (GUILayout.Button ("Variables (" + t.Variables.Count + ")", GUILayout.Height(24)))
                {
                    t.VariablesExpanded = true;
                }

                // Draw disclosure triangle
                Rect lastRect = GUILayoutUtility.GetLastRect();
                lastRect.x += 5;
                lastRect.y += 5;
                EditorGUI.Foldout(lastRect, false, "");
            }
            else
            {
                Rect listRect = new Rect();
                
                // Remove any null variables from the list
                // Can sometimes happen when upgrading to a new version of Fungus (if .meta GUID changes for a variable class)
                for (int i = t.Variables.Count - 1; i >= 0; i--)
                {
                    if (t.Variables[i] == null)
                    {
                        t.Variables.RemoveAt(i);
                    }
                }

                ReorderableListGUI.Title("Variables");
                VariableListAdaptor adaptor = new VariableListAdaptor(variablesProp, 0, w == 0 ? VariableListAdaptor.DefaultWidth : w);

                ReorderableListFlags flags = ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton;

                ReorderableListControl.DrawControlFromState(adaptor, null, flags);

                listRect = GUILayoutUtility.GetLastRect();
                
                float plusWidth = 32;
                float plusHeight = 24;

                Rect buttonRect = listRect;
                float buttonHeight = 24;
                buttonRect.x = 4;
                buttonRect.y -= buttonHeight - 1;
                buttonRect.height = buttonHeight;
                if (!Application.isPlaying)
                {
                    buttonRect.width -= 30;
                }

                if (showVariableToggleButton && GUI.Button(buttonRect, "Variables"))
                {
                    t.VariablesExpanded = false;
                }

                // Draw disclosure triangle
                Rect lastRect = buttonRect;
                lastRect.x += 5;
                lastRect.y += 5;
                
                //this is not required, seems to be legacy that is hidden in the normal reorderable
                if(showVariableToggleButton)
                    EditorGUI.Foldout(lastRect, true, "");

                Rect plusRect = listRect;
                plusRect.x += plusRect.width - plusWidth;
                plusRect.y -= plusHeight - 1;
                plusRect.width = plusWidth;
                plusRect.height = plusHeight;

                if (!Application.isPlaying && 
                    GUI.Button(plusRect, addTexture))
                {
                    GenericMenu menu = new GenericMenu ();
                    List<System.Type> types = FindAllDerivedTypes<Variable>();

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
                        addVariableInfo.flowchart = t;
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
                        info.flowchart = t;
                        info.variableType = type;

                        GUIContent typeName = new GUIContent(variableInfo.Category + "/" + variableInfo.VariableType);

                        menu.AddItem(typeName, false, AddVariable, info);
                    }

                    menu.ShowAsContext ();
                }
            }

            serializedObject.ApplyModifiedProperties();
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

        public static List<System.Type> FindAllDerivedTypes<T>()
        {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }
        
        public static List<System.Type> FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly
                .GetTypes()
                    .Where(t =>
                           t != derivedType &&
                           derivedType.IsAssignableFrom(t)
                           ).ToList();
            
        }

        /// <summary>
        /// When modifying custom editor code you can occasionally end up with orphaned editor instances.
        /// When this happens, you'll get a null exception error every time the scene serializes / deserialized.
        /// Once this situation occurs, the only way to fix it is to restart the Unity editor.
        /// As a workaround, this function detects if this editor is an orphan and deletes it. 
        /// </summary>
        protected virtual bool NullTargetCheck()
        {
            try
            {
                // The serializedObject accessor creates a new SerializedObject if needed.
                // However, this will fail with a null exception if the target object no longer exists.
                #pragma warning disable 0219
                SerializedObject so = serializedObject;
            }
            catch (System.NullReferenceException)
            {
                DestroyImmediate(this);
                return true;
            }
            
            return false;
        }
    }
}