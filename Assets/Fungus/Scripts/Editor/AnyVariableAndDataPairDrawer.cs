// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    /// <summary>
    /// Custom drawer for the AnyVaraibleAndDataPair, shows only the matching data for the targeted variable
    /// scripts.
    /// </summary>
    [CustomPropertyDrawer(typeof(Fungus.AnyVariableAndDataPair))]
    public class AnyVariableAndDataPairDrawer : PropertyDrawer
    {
        public Fungus.Flowchart lastFlowchart;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;

            var varProp = property.FindPropertyRelative("variable");

            EditorGUI.PropertyField(position, varProp, label);

            position.y += EditorGUIUtility.singleLineHeight;

            if (varProp.objectReferenceValue != null)
            {
                var varPropType = varProp.objectReferenceValue.GetType();

                var typeActionsRes = AnyVariableAndDataPair.typeActionLookup[varPropType];

                if (typeActionsRes != null)
                {
                    var targetName = "data." + typeActionsRes.DataPropName;
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