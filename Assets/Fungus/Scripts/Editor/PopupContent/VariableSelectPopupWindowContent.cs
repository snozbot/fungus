// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Reflection;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Show the variable selection window as a searchable popup
    /// </summary>
    public class VariableSelectPopupWindowContent : BasePopupWindowContent
    {
        static readonly int POPUP_WIDTH = 200, POPUP_HEIGHT = 200;
        static List<System.Type> _variableTypes;
        static List<System.Type> VariableTypes
        {
            get
            {
                if (_variableTypes == null || _variableTypes.Count == 0)
                    CacheVariableTypes();

                return _variableTypes;
            }
        }

        static void CacheVariableTypes()
        {
            var derivedType = typeof(Variable);
            _variableTypes = EditorExtensions.FindDerivedTypes(derivedType)
                .Where(x => !x.IsAbstract && derivedType.IsAssignableFrom(x))
                .ToList();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            CacheVariableTypes();
        }

        protected override void PrepareAllItems()
        {
            int i = 0;
            foreach (var item in VariableTypes)
            {
                VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(item);
                if (variableInfo != null)
                {
                    var obsAttr = item.GetCustomAttribute<System.ObsoleteAttribute>();

                    var fliStr = (variableInfo.Category.Length > 0 ? variableInfo.Category + CATEGORY_CHAR : "")
                        + (obsAttr != null ? FungusConstants.UIPrefixForDeprecated_RichText : "")
                        + variableInfo.VariableType;
                    allItems.Add(new FilteredListItem(i, fliStr));
                }

                i++;
            }
        }

        protected override void SelectByOrigIndex(int index)
        {
            AddVariable(VariableTypes[index]);
        }

        static public void DoAddVariable(Rect position, string currentHandlerName, Flowchart flowchart)
        {
            curFlowchart = flowchart;
            if (!FungusEditorPreferences.useLegacyMenus)
            {
                //new method
                VariableSelectPopupWindowContent win = new VariableSelectPopupWindowContent(currentHandlerName, POPUP_WIDTH, POPUP_HEIGHT);
                PopupWindow.Show(position, win);
            }
            //old method
            DoOlderMenu(flowchart);
        }

        static protected void DoOlderMenu(Flowchart flowchart)
        {
            GenericMenu menu = new GenericMenu();

            // Add variable types without a category
            foreach (var type in VariableTypes)
            {
                VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
                if (variableInfo == null ||
                    variableInfo.Category != "")
                {
                    continue;
                }

                GUIContent typeName = new GUIContent(variableInfo.VariableType);

                menu.AddItem(typeName, false, AddVariable, type);
            }

            // Add types with a category
            foreach (var type in VariableTypes)
            {
                VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
                if (variableInfo == null ||
                    variableInfo.Category == "")
                {
                    continue;
                }
                
                GUIContent typeName = new GUIContent(variableInfo.Category + CATEGORY_CHAR + variableInfo.VariableType);

                menu.AddItem(typeName, false, AddVariable, type);
            }

            menu.ShowAsContext();
        }

        private static Flowchart curFlowchart;

        public VariableSelectPopupWindowContent(string currentHandlerName, int width, int height)
            : base(currentHandlerName, width, height)
        {
        }

        public static void AddVariable(object obj)
        {
            AddVariable(obj, string.Empty);
        }

        public static void AddVariable(object obj, string suggestedName)
        {
            System.Type t = obj as System.Type;
            if (t == null)
            {
                return;
            }

            var flowchart = curFlowchart != null ? curFlowchart : FlowchartWindow.GetFlowchart();
            Undo.RecordObject(flowchart, "Add Variable");
            Variable newVariable = flowchart.gameObject.AddComponent(t) as Variable;
            newVariable.Key = flowchart.GetUniqueVariableKey(suggestedName);

            //if suggested exists, then insert, if not just add
            var existingVariable = flowchart.GetVariable(suggestedName);
            if (existingVariable != null)
            {
                flowchart.Variables.Insert(flowchart.Variables.IndexOf(existingVariable)+1, newVariable);
            }
            else
            {
                flowchart.Variables.Add(newVariable);
            }

            // Because this is an async call, we need to force prefab instances to record changes
            PrefabUtility.RecordPrefabInstancePropertyModifications(flowchart);
        }
    }
}