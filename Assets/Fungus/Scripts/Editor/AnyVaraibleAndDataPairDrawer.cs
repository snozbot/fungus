using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Custom drawer for the AnyVaraibleAndDataPair, shows only the matching data for the targeted variable
    /// scripts.
    /// </summary>
    [CustomPropertyDrawer(typeof(Fungus.AnyVaraibleAndDataPair))]
    public class AnyVaraibleAndDataPairDrawer : PropertyDrawer
    {
        public Fungus.Flowchart lastFlowchart;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            var varProp = property.FindPropertyRelative("variable");

            EditorGUI.PropertyField(position, varProp, label);

            position.y += EditorGUIUtility.singleLineHeight;

            if(varProp.objectReferenceValue != null)
            {
                var varPropType = varProp.objectReferenceValue.GetType();

                var typeIndex = System.Array.IndexOf(VariableInfo.AllFungusVarTypes, varPropType);

                if(typeIndex != -1)
                {
                    var targetName = "data." + AnyVariableData.PropertyNameByTypeIndex[typeIndex];
                    var dataProp = property.FindPropertyRelative(targetName);
                    if (dataProp != null)
                    {
                        EditorGUI.PropertyField(position, dataProp, new GUIContent("Data", "Data to use in pair with the above variable."));
                    }
                    else
                    {
                        EditorGUI.LabelField(position, "Cound not find property in AnyVariableData of name " + targetName);
                    }
                }
                else
                {
                    //no matching data type, oops
                    EditorGUI.LabelField(position, "Cound not find property in AnyVariableData of type " + varPropType.Name);
                }
            }
            else
            {
                //no var selected
                EditorGUI.LabelField(position, "Must select a variable before setting data.");
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2;
        }
    }
}