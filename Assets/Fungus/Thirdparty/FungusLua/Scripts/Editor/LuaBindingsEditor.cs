/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using UnityEditor.Callbacks;

namespace Fungus
{
    
    [CustomEditor (typeof(LuaBindings))]
    public class LuaBindingsEditor : Editor 
    {
        protected ReorderableList boundObjectsList;

        protected SerializedProperty allLuaEnvironmentsProp;
        protected SerializedProperty luaEnvironmentProp;
        protected SerializedProperty tableNameProp;
        protected SerializedProperty registerTypesProp;
        protected SerializedProperty boundObjectsProp;
        protected SerializedProperty showInheritedProp;

        protected string bindingHelpItem = ""; 
        protected string bindingHelpDetail = "";

        protected virtual void OnEnable()
        {
            allLuaEnvironmentsProp = serializedObject.FindProperty("allEnvironments");
            luaEnvironmentProp = serializedObject.FindProperty("luaEnvironment");
            tableNameProp = serializedObject.FindProperty("tableName");
            registerTypesProp = serializedObject.FindProperty("registerTypes");
            boundObjectsProp = serializedObject.FindProperty("boundObjects");
            showInheritedProp = serializedObject.FindProperty("showInherited");
            CreateBoundObjectsList();
        }

        protected void CreateBoundObjectsList()
        {
            boundObjectsList = new ReorderableList(serializedObject, boundObjectsProp, true, true, true, true);

            boundObjectsList.drawElementCallback =  
                (Rect rect, int index, bool isActive, bool isFocused) => {
                var element = boundObjectsList.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;

                float widthA = rect.width * 0.25f;
                float widthB = rect.width * 0.75f * 0.5f;
                float widthC = rect.width * 0.75f * 0.5f;

                SerializedProperty keyProp = element.FindPropertyRelative("key");
                SerializedProperty objectProp = element.FindPropertyRelative("obj");

                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, widthA - 5, EditorGUIUtility.singleLineHeight),
                    keyProp, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    // Force the key to be a valid Lua variable name
                    LuaBindings luaBindings = target as LuaBindings;
                    keyProp.stringValue = GetUniqueKey(luaBindings, keyProp.stringValue, index);
                }

                EditorGUI.BeginChangeCheck();

                EditorGUI.PropertyField(
                    new Rect(rect.x + widthA, rect.y, widthB - 5, EditorGUIUtility.singleLineHeight),
                    objectProp, GUIContent.none);

                if (EditorGUI.EndChangeCheck())
                {
                    // Use the object name as the key
                    string keyName = objectProp.objectReferenceValue.name;
                    LuaBindings luaBindings = target as LuaBindings;
                    element.FindPropertyRelative("key").stringValue = GetUniqueKey(luaBindings, keyName.ToLower(), index);

                    // Auto select any Flowchart component in the object
                    GameObject go = objectProp.objectReferenceValue as GameObject;
                    if (go != null)
                    {
                        Component flowchart = go.GetComponent("Fungus.Flowchart");
                        if (flowchart != null)
                        {
                            SerializedProperty componentProp = element.FindPropertyRelative("component");
                            componentProp.objectReferenceValue = flowchart;
                        }
                    }
                }

                if (objectProp.objectReferenceValue != null)
                {         
                    GameObject go = objectProp.objectReferenceValue as GameObject;
                    if (go != null)
                    {
                        SerializedProperty componentProp = element.FindPropertyRelative("component");

                        int selected = 0;
                        List<string> options = new List<string>();
                        options.Add("<GameObject>");

                        int count = 1;
                        Component[] componentList = go.GetComponents<Component>();
                        foreach (Component component in componentList)
                        {
                            if (componentProp.objectReferenceValue == component)
                            {
                                selected = count;
                            }

                            if (component == null ||
                                component.GetType() == null)
                            {
                                // Missing script?
                                continue;
                            }

                            string componentName = component.GetType().ToString().Replace("UnityEngine.", "");
                            options.Add(componentName);

                            count++;
                        }

                        int i = EditorGUI.Popup(
                            new Rect(rect.x + widthA + widthB, rect.y, widthC, EditorGUIUtility.singleLineHeight),
                            selected,
                            options.ToArray());
                        if (i == 0)
                        {
                            componentProp.objectReferenceValue = null;
                        }
                        else
                        {
                            componentProp.objectReferenceValue = componentList[i - 1];
                        }
                    }                            
                }

                boundObjectsList.onAddCallback = (ReorderableList l) => {  
                    // Add a new item. This copies last item in the list, so clear new items values.
                    boundObjectsProp.InsertArrayElementAtIndex(boundObjectsProp.arraySize);
                    SerializedProperty newItem = boundObjectsProp.GetArrayElementAtIndex(boundObjectsProp.arraySize - 1);
                    newItem.FindPropertyRelative("key").stringValue = "";
                    newItem.FindPropertyRelative("obj").objectReferenceValue = null;
                    newItem.FindPropertyRelative("component").objectReferenceValue = null;
                };
            };

            boundObjectsList.drawHeaderCallback = (Rect rect) => {

                float widthA = rect.width * 0.25f;
                float widthB = rect.width * 0.75f * 0.5f;
                float widthC = rect.width * 0.75f * 0.5f;

                EditorGUI.LabelField(new Rect(rect.x+ 12, rect.y, widthA, EditorGUIUtility.singleLineHeight), "Key");
                EditorGUI.LabelField(new Rect(rect.x + widthA + 12, rect.y, widthB, EditorGUIUtility.singleLineHeight), "Object");
                EditorGUI.LabelField(new Rect(rect.x + widthA + widthB + 6, rect.y, widthC, EditorGUIUtility.singleLineHeight), "Component");
            };
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(allLuaEnvironmentsProp);
            if (!allLuaEnvironmentsProp.boolValue)
            {
                EditorGUILayout.PropertyField(luaEnvironmentProp);
            }

            EditorGUILayout.PropertyField(tableNameProp);
            EditorGUILayout.PropertyField(registerTypesProp);

            EditorGUILayout.LabelField("Object Bindings");

            boundObjectsList.DoLayoutList();
            EditorGUILayout.Space();

            ShowBindingMemberInfo();

            // Update the bound types on every tick to make sure they're up to date.
            // This could be a bit heavy on performance if the bound object list is long, but
            // I couldn't get it to work reliably by only updating when the list has changed.
            // This only happens when inspecting a Fungus Bindings component so I think it'll be ok.
            PopulateBoundTypes(target as LuaBindings, serializedObject);

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void ShowBindingMemberInfo()
        {
            List<string> items = new List<string>();
            items.Add("<Select a binding member>");

            List<string> details = new List<string>();
            details.Add("");

            LuaBindings luaBindings = target as LuaBindings;
            foreach (LuaBindings.BoundObject boundObject in luaBindings.boundObjects)
            {
                UnityEngine.Object inspectObject = boundObject.obj;
                if (boundObject.component != null)
                {
                    inspectObject = boundObject.component;
                }

                // Ignore empty entries
                if (inspectObject == null ||
                    boundObject.key.Length == 0)
                {
                    continue;
                }

                BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public; 
                if (!showInheritedProp.boolValue)
                {
                    flags |= BindingFlags.DeclaredOnly;
                }

                // Show info for fields and properties
                MemberInfo[] memberInfos = inspectObject.GetType().GetMembers(flags);
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.MemberType != MemberTypes.Field &&
                        memberInfo.MemberType != MemberTypes.Property)
                    {
                        continue;
                    }

                    string item = boundObject.key + "/" + memberInfo.Name;
                    items.Add(item);

                    string detail = "";
                    detail += "Binding Key: " + boundObject.key + "\n";
                    detail += "Bound Object: " + boundObject.obj.name + "\n";
                    if (boundObject.component != null)
                    {
                        detail += "Bound Component: " + boundObject.component.GetType() + "\n";
                    }

                    if (memberInfo.MemberType == MemberTypes.Field)
                    {
                        detail += "Field Name: " + memberInfo.Name + "\n";
                        detail += "Field Type: " + (memberInfo as FieldInfo).FieldType;
                    }
                    else if (memberInfo.MemberType == MemberTypes.Property)
                    {
                        detail += "Property Name: " + memberInfo.Name + "\n";
                        detail += "Property Type: " + (memberInfo as PropertyInfo).PropertyType;
                    }

                    details.Add(detail);
                }

                // Show info for methods
                foreach (MemberInfo memberInfo in memberInfos)
                {
                    if (memberInfo.MemberType != MemberTypes.Method)
                    {
                        continue;
                    }

                    MethodInfo methodInfo = memberInfo as MethodInfo;
                    if (methodInfo.IsGenericMethod)
                    {
                        continue;
                    }

                    // Build list of parameters
                    string param = "";
                    foreach (ParameterInfo pi in methodInfo.GetParameters() )
                    {
                        if (param == "")
                        {
                            param += pi.Name;
                        }
                        else
                        {
                            param += ", " + pi.Name;
                        }
                    }
                    param = "(" + param + ")";

                    string item = boundObject.key + "/" + methodInfo.Name + param;
                    items.Add(item);

                    string detail = "";
                    detail += "Binding Key: " + boundObject.key + "\n";
                    detail += "Bound Object: " + boundObject.obj.name + "\n";
                    if (boundObject.component != null)
                    {
                        detail += "Bound Component: " + boundObject.component.GetType() + "\n";
                    }

                    detail += "Method Name: " + methodInfo.Name + "\n";
                    foreach (ParameterInfo pi in methodInfo.GetParameters() )
                    {
                        detail += "Parameter: " + pi.Name + " (" + pi.ParameterType.ToString() + ")\n";
                    }
                    detail += "Return Type : " + methodInfo.ReturnType.ToString();
 
                    details.Add(detail);
                }
            }

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel(new GUIContent("Member Info", "Display member info for the selected binding"));

            GUILayout.FlexibleSpace();

            // Display the popup list of bound members
            int selectedIndex = EditorGUILayout.Popup(0, items.ToArray());
            if (selectedIndex > 0)
            {
                bindingHelpItem = items[selectedIndex].Replace("/", ".");
                bindingHelpDetail = details[selectedIndex];
            }
                
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(showInheritedProp);

            // Show help info for currently selected item
            if (bindingHelpItem != "")
            {
                EditorGUILayout.HelpBox(bindingHelpDetail, MessageType.Info);

                EditorGUILayout.PrefixLabel(new GUIContent("Lua Usage", "A copyable example of how this item could be used in a Lua script."));

                GUIStyle textAreaStyle = EditorStyles.textField;
                textAreaStyle.wordWrap = true;

                EditorGUILayout.TextArea(bindingHelpItem, textAreaStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 3));
            }
        }

        /// <summary>
        /// Returns a new binding key that is guaranteed to be a valid Lua variable name and
        /// not to clash with any existing binding in the list.
        /// </summary>
        protected virtual string GetUniqueKey(LuaBindings luaBindings, string originalKey, int ignoreIndex = -1)
        {
            string baseKey = originalKey;

            // Only letters and digits allowed
            char[] arr = baseKey.Where(c => (char.IsLetterOrDigit(c) || c == '_')).ToArray(); 
            baseKey = new string(arr);

            // No leading digits allowed
            baseKey = baseKey.TrimStart('0','1','2','3','4','5','6','7','8','9');

            // No empty keys allowed
            if (baseKey.Length == 0)
            {
                baseKey = "object";
            }

            // Build a hash of all keys currently in use
            HashSet<string> keyhash = new HashSet<string>();
            for (int i = 0; i < luaBindings.boundObjects.Count; ++i)
            {
                if (i == ignoreIndex)
                {
                    continue;
                }

                keyhash.Add(luaBindings.boundObjects[i].key);
            }

            // Append a suffix to make the key unique
            string key = baseKey;
            int suffix = 0;
            while (keyhash.Contains(key))
            {
                suffix++;
                key = baseKey + suffix;
            }

            return key;
        }

        [DidReloadScripts()]
        protected static void DidReloadScripts()
        {
            LuaBindings[] luaBindingsList = GameObject.FindObjectsOfType<LuaBindings>();
            foreach (LuaBindings luaBindings in luaBindingsList)
            {
                SerializedObject so = new SerializedObject(luaBindings);
                so.Update();

                PopulateBoundTypes(luaBindings, so);

                so.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Update the list of bound types on the LuaBindings object.
        /// </summary>
        protected static void PopulateBoundTypes(LuaBindings luaBindings, SerializedObject so)
        {
            // Use a temp HashSet to store the list of types.
            // The final list is stored as a list of type strings.
            HashSet<System.Type> typeSet = new HashSet<System.Type>();
            foreach (LuaBindings.BoundObject boundObject in luaBindings.boundObjects)
            {
                if (boundObject.obj == null)
                {
                    continue;
                }

                AddAllSubTypes(typeSet, boundObject.obj.GetType());

                if (boundObject.component != null)
                {
                    AddAllSubTypes(typeSet, boundObject.component.GetType());
                }
            }
                
            // Store the final list of types in the luaBindings object 
            SerializedProperty boundTypesProp = so.FindProperty("boundTypes");
            boundTypesProp.ClearArray();
            int index = 0;
            foreach (System.Type t in typeSet)
            {
                boundTypesProp.InsertArrayElementAtIndex(index);
                SerializedProperty element = boundTypesProp.GetArrayElementAtIndex(index);
                element.stringValue = t.AssemblyQualifiedName;
                index++;
            }
        }

        /// <summary>
        /// Adds the type to the set of types, and then uses reflection to add
        /// all public fields, properties and methods to the set of types.
        /// </summary>
        protected static void AddAllSubTypes(HashSet<System.Type> typeSet, System.Type t)
        {
            AddSubType(typeSet, t);

            MemberInfo[] memberInfos = t.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    FieldInfo fieldInfo = memberInfo as FieldInfo;
                    AddSubType(typeSet, fieldInfo.FieldType);
                }
                else if (memberInfo.MemberType == MemberTypes.Property)
                {
                    PropertyInfo propertyInfo = memberInfo as PropertyInfo;
                    AddSubType(typeSet, propertyInfo.PropertyType);
                }
                else if (memberInfo.MemberType == MemberTypes.Method)
                {
                    MethodInfo methodInfo = memberInfo as MethodInfo;
                    if (methodInfo.IsGenericMethod)
                    {
                        continue;
                    }

                    if (methodInfo.ReturnType != typeof(void))
                    {
                        AddSubType(typeSet, methodInfo.ReturnType);
                    }

                    foreach (ParameterInfo pi in methodInfo.GetParameters() )
                    {
                        AddSubType(typeSet, pi.ParameterType);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a single type to the type set.
        /// IEnumerable and IEnumerator types are handled specially.
        /// </summary>
        protected static void AddSubType(HashSet<System.Type> typeSet, System.Type t)
        {
            // MoonSharp handles IEnumerator and IEnumerable types automatically, so just
            // register the generic type used.
            if (typeof(IEnumerable).IsAssignableFrom(t) ||
                typeof(IEnumerator).IsAssignableFrom(t))
            {
                System.Type[] genericArguments = t.GetGenericArguments();
                if (genericArguments.Count() == 1)
                {
                    System.Type containedType = genericArguments[0];

                    // The generic type could itself be an IEnumerator or IEnumerable, so
                    // recursively check for this case.
                    AddSubType(typeSet, containedType);
                }
            }
            else if (t != typeof(System.Object))
            {
                // Non-IEnumerable/IEnumerator types will be registered.
                typeSet.Add(t);
            }
        }

   }

}