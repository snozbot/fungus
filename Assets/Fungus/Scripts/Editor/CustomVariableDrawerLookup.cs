// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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
        public static Dictionary<System.Type, System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty, GUIContent>> typeToDrawer =
            new Dictionary<System.Type, System.Action<Rect, UnityEditor.SerializedProperty, GUIContent>>()
            {
                {
                    typeof(QuaternionVariable),
                    (rect, valueProp, con) => {valueProp.quaternionValue = UnityEngine.Quaternion.Euler(UnityEditor.EditorGUI.Vector3Field(rect, con, valueProp.quaternionValue.eulerAngles)); }
                },
                {
                    typeof(Vector4Variable),
                    (rect, valueProp, con) => {valueProp.vector4Value = UnityEditor.EditorGUI.Vector4Field(rect, con, valueProp.vector4Value); }
                },
                {
                    typeof(Matrix4x4Variable),
                    (rect, valueProp, con) => {UnityEditor.EditorGUI.PropertyField(rect, valueProp, con, true); }
                },
            };

        /// <summary>
        /// Called by editors that want a single line variable property drawn
        /// </summary>
        /// <param name="type"></param>
        /// <param name="rect"></param>
        /// <param name="prop"></param>
        public static void DrawCustomOrPropertyField(System.Type type, Rect rect, SerializedProperty prop, GUIContent label)
        {
            System.Action<UnityEngine.Rect, UnityEditor.SerializedProperty, GUIContent> drawer = null;
            //delegate actual drawing to the variableInfo
            var foundDrawer = typeToDrawer.TryGetValue(type, out drawer);
            if (foundDrawer)
            {
                drawer(rect, prop, label);
            }
            else
            {
                EditorGUI.PropertyField(rect, prop, label);
            }
        }
    }
}