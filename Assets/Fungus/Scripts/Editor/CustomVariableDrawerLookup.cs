using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Fungus Variables are drawn via EditorGUI.Property by default, however, some types may require a custom replacement.
    /// The most common example is a Quaternion, Unity does not show you a quaternion as 4 floats, it shows you
    /// the euler angles, we also want to do that here
    /// 
    /// This class is delegated to by editors to draw the actual variable property line.
    /// </summary>
    public static class CustomVariableDrawerLookup
    {
        //If you create new types that require custom singleline drawers, add them here
        public static Dictionary<System.Type, System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty>> typeToDrawer =
            new Dictionary<System.Type, System.Action<Rect, UnityEditor.SerializedProperty>>()
            {
                { 
                    typeof(QuaternionVariable),
                    (rect, valueProp) => {valueProp.quaternionValue = UnityEngine. Quaternion.Euler(UnityEditor.EditorGUI.Vector3Field(rect, new UnityEngine.GUIContent(""), valueProp.quaternionValue.eulerAngles)); }
                },
                {
                    typeof(Vector4Variable),
                    (rect, valueProp) => {valueProp.vector4Value = UnityEditor.EditorGUI.Vector4Field(rect, new UnityEngine.GUIContent(""), valueProp.vector4Value); }
                },
            };


        public static bool GetDrawer(Type type, out Action<Rect, SerializedProperty> drawer)
        {
            return typeToDrawer.TryGetValue(type, out drawer);
        }

        /// <summary>
        /// Called by editors that want a single line variable property drawn
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rect"></param>
        /// <param name="prop"></param>
        public static void DrawCustomOrPropertyField(Type type, Rect rect, SerializedProperty prop)
        {
            //delegate actual drawing to the variableInfo
            var foundDrawer = typeToDrawer.TryGetValue(type, out System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty> drawer);
            if (foundDrawer)
            {
                drawer(rect, prop);
            }
            else
            {
                EditorGUI.PropertyField(rect, prop, new GUIContent(""));
            }
        }
    }
}