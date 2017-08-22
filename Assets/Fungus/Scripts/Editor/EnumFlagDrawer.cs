//from http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer

//placed in fungus namespace to avoid collisions with your own

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Fungus
{
    [CustomPropertyDrawer(typeof(EnumFlagAttribute))]
    public class EnumFlagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnumFlagAttribute flagSettings = (EnumFlagAttribute)attribute;
            Enum targetEnum = GetBaseProperty<Enum>(property);

            string propName = flagSettings.enumName;
            if (string.IsNullOrEmpty(propName))
                propName = property.name;

            EditorGUI.BeginProperty(position, label, property);
            Enum enumNew = EditorGUI.EnumMaskField(position, propName, targetEnum);
            property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
            EditorGUI.EndProperty();
        }

        static T GetBaseProperty<T>(SerializedProperty prop)
        {
            // Separate the steps it takes to get to this property
            string[] separatedPaths = prop.propertyPath.Split('.');

            // Go down to the root of this serialized property
            System.Object reflectionTarget = prop.serializedObject.targetObject as object;
            // Walk down the path to get the target object
            foreach (var path in separatedPaths)
            {
                var t = reflectionTarget.GetType();
                //with support for private types via https://gist.github.com/ChemiKhazi/11395776
                FieldInfo fieldInfo = t.GetField(path, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                reflectionTarget = fieldInfo.GetValue(reflectionTarget);
            }
            return (T)reflectionTarget;
        }
    }
}