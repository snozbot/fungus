// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEditor;
using System;
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;

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
        public Flowchart TargetFlowchart { get; private set; }

        private float[] itemWidths = new float[4];
        private Rect[] itemRects = new Rect[4];

        public SerializedProperty this[int index]
        {
            get { return _arrayProperty.GetArrayElementAtIndex(index); }
        }

        public Variable GetVarAt(int index)
        {
            if (list.list != null)
                return list.list[index] as Variable;
            else
                return this[index].objectReferenceValue as Variable;
        }

        public void SetVarAt(int index, Variable v)
        {
            if (list.list != null)
            {
                list.list[index] = v;
            }
            else
            {
                this[index].objectReferenceValue = v;
            }
        }

        public VariableListAdaptor(SerializedProperty arrayProperty, Flowchart _targetFlowchart)
        {
            if (arrayProperty == null)
                throw new ArgumentNullException("Array property was null.");
            if (!arrayProperty.isArray)
                throw new InvalidOperationException("Specified serialized propery is not an array.");

            this.TargetFlowchart = _targetFlowchart;
            this.fixedItemHeight = 0;
            this._arrayProperty = arrayProperty;
            this.widthOfList = widthOfList - ScrollSpacer;

            list = new ReorderableList(arrayProperty.serializedObject, arrayProperty, true, false, true, true);
            list.drawElementCallback = DrawItem;
            list.onRemoveCallback = RemoveItem;
            //list.onAddCallback = AddButton;
            list.onAddDropdownCallback = AddDropDown;
            list.onRemoveCallback = RemoveItem;
            list.elementHeightCallback = GetElementHeight;
        }

        private float GetElementHeight(int index)
        {
            return /*EditorGUI.GetPropertyHeight(this[index], null, true) +*/ EditorGUIUtility.singleLineHeight;
        }

        private void RemoveItem(ReorderableList list)
        {
            int index = list.index;
            // Remove the Fungus Variable component
            Variable variable = this[index].objectReferenceValue as Variable;
            Undo.DestroyObjectImmediate(variable);
        }

        private void AddDropDown(Rect buttonRect, ReorderableList list)
        {
            Event.current.Use();
            VariableSelectPopupWindowContent.DoAddVariable(buttonRect, "", TargetFlowchart);
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

        public void DrawVarList(int w)
        {
            //we want to eat the throw that occurs when switching back to editor from play
            try
            {
                if (_arrayProperty == null || _arrayProperty.serializedObject == null)
                    return;


                _arrayProperty.serializedObject.Update();
                this.widthOfList = (w == 0 ? VariableListAdaptor.DefaultWidth : w) - ScrollSpacer;

                int width = widthOfList;
                int totalRatio = DefaultWidth;

                itemWidths[0] = (80.0f / totalRatio) * width;
                itemWidths[1] = (100.0f / totalRatio) * width;
                itemWidths[2] = (140.0f / totalRatio) * width;
                itemWidths[3] = (60.0f / totalRatio) * width;

                if (GUILayout.Button("Variables"))
                {
                    _arrayProperty.isExpanded = !_arrayProperty.isExpanded;
                }

                if (_arrayProperty.isExpanded)
                {
                    list.DoLayoutList();
                }
                _arrayProperty.serializedObject.ApplyModifiedProperties();
            }
            catch (Exception)
            {
            }
        }

        public void DrawItem(Rect position, int index, bool selected, bool focused)
        {
            Variable variable = GetVarAt(index);// this[index].objectReferenceValue as Variable;

            if (variable == null)
            {
                return;
            }

            if(Event.current.type == EventType.ContextClick && position.Contains(Event.current.mousePosition))
            {
                DoRightClickMenu(index);
            }

            for (int i = 0; i < 4; ++i)
            {
                itemRects[i] = position;
                itemRects[i].width = itemWidths[i] - 5;

                for (int j = 0; j < i; ++j)
                {
                    itemRects[i].x += itemWidths[j];
                }
            }

            VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(variable.GetType());
            if (variableInfo == null)
            {
                return;
            }

            var flowchart = TargetFlowchart;
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
                    highlight = flowchart.SelectedBlock.ActiveCommand.IsVariableReferenced(variable);
                }
                else if (!Application.isPlaying && flowchart.SelectedCommands.Count > 0)
                {
                    foreach (Command selectedCommand in flowchart.SelectedCommands)
                    {
                        if (selectedCommand == null)
                        {
                            continue;
                        }

                        if (selectedCommand.IsVariableReferenced(variable))
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
            SerializedObject variableObject = new SerializedObject(variable);

            variableObject.Update();

            GUI.Label(itemRects[0], variableInfo.VariableType);

            SerializedProperty keyProp = variableObject.FindProperty("key");
            SerializedProperty defaultProp = variableObject.FindProperty("value");
            SerializedProperty scopeProp = variableObject.FindProperty("scope");


            EditorGUI.BeginChangeCheck();
            key = EditorGUI.TextField(itemRects[1], variable.Key);
            if (EditorGUI.EndChangeCheck())
            {
                keyProp.stringValue = flowchart.GetUniqueVariableKey(key, variable);
            }

            bool isGlobal = scopeProp.enumValueIndex == (int)VariableScope.Global;

            var prevEnabled = GUI.enabled;
            if (isGlobal && Application.isPlaying)
            {
                var res = FungusManager.Instance.GlobalVariables.GetVariable(keyProp.stringValue);
                if (res != null)
                {
                    SerializedObject globalValue = new SerializedObject(res);
                    var globalValProp = globalValue.FindProperty("value");


                    GUI.enabled = false;
                    defaultProp = globalValProp;
                }
            }


            //variable.DrawProperty(rects[2], defaultProp, variableInfo);
            VariableDrawProperty(variable, itemRects[2], defaultProp, variableInfo);

            GUI.enabled = prevEnabled;


            scope = (VariableScope)EditorGUI.EnumPopup(itemRects[3], variable.Scope);
            scopeProp.enumValueIndex = (int)scope;

            variableObject.ApplyModifiedProperties();

            GUI.backgroundColor = Color.white;
        }
        
        public static void VariableDrawProperty(Variable variable, Rect rect, SerializedProperty valueProp, VariableInfoAttribute info)
        {
            if (valueProp == null)
            {
                EditorGUI.LabelField(rect, "N/A");
            }
            else if (info.IsPreviewedOnly)
            {
                EditorGUI.LabelField(rect, variable.ToString());
            }
            else 
            {
                CustomVariableDrawerLookup.DrawCustomOrPropertyField(variable.GetType(), rect, valueProp, GUIContent.none);
            }
        }

        private void DoRightClickMenu(int index)
        {
            var v = GetVarAt(index);

            GenericMenu commandMenu = new GenericMenu();
            commandMenu.AddItem(new GUIContent("Remove"), false, () => {list.index = index; RemoveItem(list); });
            commandMenu.AddItem(new GUIContent("Duplicate"), false, () => VariableSelectPopupWindowContent.AddVariable(v.GetType(), v.Key));
            commandMenu.AddItem(new GUIContent("Find References"), false, () => FindUsage(GetVarAt(index)));
            commandMenu.AddSeparator("");
            commandMenu.AddItem(new GUIContent("Sort by Name"), false, () => SortBy(x =>  x.Key));
            commandMenu.AddItem(new GUIContent("Sort by Type"), false, () => SortBy(x => x.GetType().Name));
            commandMenu.AddItem(new GUIContent("Sort by Value"), false, () => SortBy(x => x.GetValue()));
            commandMenu.ShowAsContext();
        }

        private void SortBy<TKey>(Func<Variable, TKey> orderFunc)
        {
            List<Variable> vars = new List<Variable>();
            for (int i = 0; i < list.count; i++)
            {
                vars.Add(GetVarAt(i));
            }

            vars = vars.OrderBy(orderFunc).ToList();

            for (int i = 0; i < list.count; i++)
            {
                SetVarAt(i, vars[i]);
            }

            _arrayProperty.serializedObject.ApplyModifiedProperties();
        }

        private void FindUsage(Variable variable)
        {                
            var varRefs = EditorExtensions.FindObjectsOfInterface<IVariableReference>()
                .Where(x => x.HasReference(variable))
                .Select(x => x.GetLocationIdentifier()).ToArray(); ;

            string varRefString = variable.Key;

            if (varRefs != null && varRefs.Length > 0)
            {
                varRefString += " referenced in " + varRefs.Length.ToString() + " places:\n";

                varRefString += string.Join("\n - ", varRefs);
            }
            else
            {
                varRefString += ", found no references.";
            }

            Debug.Log(varRefString);
        }
    }
}

