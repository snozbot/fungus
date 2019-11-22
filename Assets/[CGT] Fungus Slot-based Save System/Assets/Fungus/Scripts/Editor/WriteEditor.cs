// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Write))]
    public class WriteEditor : CommandEditor
    {
        public static bool showTagHelp;

        protected SerializedProperty textObjectProp;
        protected SerializedProperty textProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty clearTextProp;
        protected SerializedProperty textColorProp;
        protected SerializedProperty setAlphaProp;
        protected SerializedProperty setColorProp;
        protected SerializedProperty waitUntilFinishedProp;

        public static void DrawTagHelpLabel()
        {
            string tagsText = "";
            tagsText += "\n";
            tagsText += TextTagParser.GetTagHelp();

            float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
        }

        public override void OnEnable()
        {
            base.OnEnable();

            textObjectProp = serializedObject.FindProperty("textObject");
            textProp = serializedObject.FindProperty("text");
            descriptionProp = serializedObject.FindProperty("description");
            clearTextProp = serializedObject.FindProperty("clearText");
            textColorProp = serializedObject.FindProperty("textColor");
            setAlphaProp = serializedObject.FindProperty("setAlpha");
            setColorProp = serializedObject.FindProperty("setColor");
            waitUntilFinishedProp = serializedObject.FindProperty("waitUntilFinished");
        }
        
        public override void DrawCommandGUI() 
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(textObjectProp);
            EditorGUILayout.PropertyField(textProp);
            EditorGUILayout.PropertyField(descriptionProp);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Text Tag Help", "View available tags"), new GUIStyle(EditorStyles.miniButton)))
            {
                showTagHelp = !showTagHelp;
            }
            EditorGUILayout.EndHorizontal();

            if (showTagHelp)
            {
                DrawTagHelpLabel();
            }

            EditorGUILayout.PropertyField(clearTextProp);

            EditorGUILayout.PropertyField(textColorProp);
            switch ((TextColor)textColorProp.enumValueIndex)
            {
            case TextColor.Default:
                break;
            case TextColor.SetVisible:
                break;
            case TextColor.SetAlpha:
                EditorGUILayout.PropertyField(setAlphaProp);
                break;
            case TextColor.SetColor:
                EditorGUILayout.PropertyField(setColorProp);
                break;
            }

            EditorGUILayout.PropertyField(waitUntilFinishedProp);

            serializedObject.ApplyModifiedProperties();
        }
    }    
}