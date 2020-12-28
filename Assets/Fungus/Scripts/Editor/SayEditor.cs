// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Say))]
    public class SayEditor : CommandEditor
    {
        public static bool showTagHelp;
        public Texture2D blackTex;
        
        public static void DrawTagHelpLabel()
        {
            string tagsText = TextTagParser.GetTagHelp();

            if (CustomTag.activeCustomTags.Count > 0)
            {
                tagsText += "\n\n\t-------- CUSTOM TAGS --------";
                List<Transform> activeCustomTagGroup = new List<Transform>();
                foreach (CustomTag ct in CustomTag.activeCustomTags)
                {
                    if(ct.transform.parent != null)
                    {
                        if (!activeCustomTagGroup.Contains(ct.transform.parent.transform))
                        {
                            activeCustomTagGroup.Add(ct.transform.parent.transform);
                        }
                    }
                    else
                    {
                        activeCustomTagGroup.Add(ct.transform);
                    }
                }
                foreach(Transform parent in activeCustomTagGroup)
                {
                    string tagName = parent.name;
                    string tagStartSymbol = "";
                    string tagEndSymbol = "";
                    CustomTag parentTag = parent.GetComponent<CustomTag>();
                    if (parentTag != null)
                    {
                        tagName = parentTag.name;
                        tagStartSymbol = parentTag.TagStartSymbol;
                        tagEndSymbol = parentTag.TagEndSymbol;
                    }
                    tagsText += "\n\n\t" + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    foreach(Transform child in parent)
                    {
                        tagName = child.name;
                        tagStartSymbol = "";
                        tagEndSymbol = "";
                        CustomTag childTag = child.GetComponent<CustomTag>();
                        if (childTag != null)
                        {
                            tagName = childTag.name;
                            tagStartSymbol = childTag.TagStartSymbol;
                            tagEndSymbol = childTag.TagEndSymbol;
                        }
                            tagsText += "\n\t      " + tagStartSymbol + " " + tagName + " " + tagEndSymbol;
                    }
                }
            }
            tagsText += "\n";
            float pixelHeight = EditorStyles.miniLabel.CalcHeight(new GUIContent(tagsText), EditorGUIUtility.currentViewWidth);
            EditorGUILayout.SelectableLabel(tagsText, GUI.skin.GetStyle("HelpBox"), GUILayout.MinHeight(pixelHeight));
        }
        
        protected SerializedProperty characterProp;
        protected SerializedProperty portraitProp;
        protected SerializedProperty storyTextProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty voiceOverClipProp;
        protected SerializedProperty showAlwaysProp;
        protected SerializedProperty showCountProp;
        protected SerializedProperty extendPreviousProp;
        protected SerializedProperty fadeWhenDoneProp;
        protected SerializedProperty waitForClickProp;
        protected SerializedProperty stopVoiceoverProp;
        protected SerializedProperty setSayDialogProp;
        protected SerializedProperty waitForVOProp;

        public override void OnEnable()
        {
            base.OnEnable();

            characterProp = serializedObject.FindProperty("character");
            portraitProp = serializedObject.FindProperty("portrait");
            storyTextProp = serializedObject.FindProperty("storyText");
            descriptionProp = serializedObject.FindProperty("description");
            voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
            showAlwaysProp = serializedObject.FindProperty("showAlways");
            showCountProp = serializedObject.FindProperty("showCount");
            extendPreviousProp = serializedObject.FindProperty("extendPrevious");
            fadeWhenDoneProp = serializedObject.FindProperty("fadeWhenDone");
            waitForClickProp = serializedObject.FindProperty("waitForClick");
            stopVoiceoverProp = serializedObject.FindProperty("stopVoiceover");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
            waitForVOProp = serializedObject.FindProperty("waitForVO");

            if (blackTex == null)
            {
                blackTex = CustomGUI.CreateBlackTexture();
            }
        }
        
        protected virtual void OnDisable()
        {
            DestroyImmediate(blackTex);
        }

        public override void DrawCommandGUI() 
        {
            serializedObject.Update();

            bool showPortraits = false;
            CommandEditor.ObjectField<Character>(characterProp,
                                                new GUIContent("Character", "Character that is speaking"),
                                                new GUIContent("<None>"),
                                                Character.ActiveCharacters);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(" ");
            characterProp.objectReferenceValue = (Character) EditorGUILayout.ObjectField(characterProp.objectReferenceValue, typeof(Character), true);
            EditorGUILayout.EndHorizontal();

            Say t = target as Say;

            // Only show portrait selection if...
            if (t._Character != null &&              // Character is selected
                t._Character.Portraits != null &&    // Character has a portraits field
                t._Character.Portraits.Count > 0 )   // Selected Character has at least 1 portrait
            {
                showPortraits = true;    
            }

            if (showPortraits) 
            {
                CommandEditor.ObjectField<Sprite>(portraitProp, 
                                                  new GUIContent("Portrait", "Portrait representing speaking character"), 
                                                  new GUIContent("<None>"),
                                                  t._Character.Portraits);
            }
            else
            {
                if (!t.ExtendPrevious)
                {
                    t.Portrait = null;
                }
            }

            #region Character limits

            EditorGUILayout.PropertyField(storyTextProp);

            Color editorColor = GUI.color; // cache the GUI colour for later.

            int storyLength = -1;
            int storyMaxLength = 0;

            BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic; // get non-public properties (Protected)

            // fetch the field for the story text and calculate length from it. 
            // This is messy, but it's the only clear way to get the properties from serialized data
            var targetObject = storyTextProp.serializedObject.targetObject;
            var targetObjectClassType = targetObject.GetType();
            var storyText = targetObjectClassType.GetField(storyTextProp.propertyPath, bindingFlags);
            if (storyText != null)
            {
                string value = (string)storyText.GetValue(targetObject); 
                storyLength = value.Length; //assign the story text length based on the retrieved story text
            }

            // fetch the story max lenth
            SerializedProperty storyLimitProp = serializedObject.FindProperty("storyMaxLength");
            var storyLimit = targetObjectClassType.GetField(storyLimitProp.propertyPath, bindingFlags);
            if (storyLimit != null)
            {
                storyMaxLength = (int)storyLimit.GetValue(targetObject);
            }

            // the only time this will be 0 is if the say dialog has no limit. Null dialogs result in a reference from the default/prefab dialog.
            if (storyMaxLength != 0)
            {
                // fetch dialog name to display. This will show the user what dialog will be used for that command
                // This way, if they're using a bunch of dialogs, it can be easier to track
                string dialogName = string.Empty;
                SerializedProperty sayDialogName = serializedObject.FindProperty("sayDialogName");
                var _dialogName = targetObjectClassType.GetField(sayDialogName.propertyPath, bindingFlags);
                if (_dialogName != null)
                {
                    dialogName = (string)_dialogName.GetValue(targetObject);
                }

                // change GUI colour
                if (storyLength >= storyMaxLength - 15) GUI.color = Color.yellow;
                if (storyLength > storyMaxLength) GUI.color = Color.red;

                EditorGUILayout.LabelField(storyLength + " / " + storyMaxLength + " (" + dialogName + ")"); //display the character counter and dialog name
                GUI.color = editorColor; //reset colour to the cached colour.
            }

            GUILayout.FlexibleSpace();
            #endregion 

            EditorGUILayout.PropertyField(descriptionProp);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PropertyField(extendPreviousProp);

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("Tag Help", "View available tags"), new GUIStyle(EditorStyles.miniButton)))
            {
                showTagHelp = !showTagHelp;
            }
            EditorGUILayout.EndHorizontal();
            
            if (showTagHelp)
            {
                DrawTagHelpLabel();
            }
            
            EditorGUILayout.Separator();
            
            EditorGUILayout.PropertyField(voiceOverClipProp, 
                                          new GUIContent("Voice Over Clip", "Voice over audio to play when the text is displayed"));

            EditorGUILayout.PropertyField(showAlwaysProp);
            
            if (showAlwaysProp.boolValue == false)
            {
                EditorGUILayout.PropertyField(showCountProp);
            }

            GUIStyle centeredLabel = new GUIStyle(EditorStyles.label);
            centeredLabel.alignment = TextAnchor.MiddleCenter;
            GUIStyle leftButton = new GUIStyle(EditorStyles.miniButtonLeft);
            leftButton.fontSize = 10;
            leftButton.font = EditorStyles.toolbarButton.font;
            GUIStyle rightButton = new GUIStyle(EditorStyles.miniButtonRight);
            rightButton.fontSize = 10;
            rightButton.font = EditorStyles.toolbarButton.font;

            EditorGUILayout.PropertyField(fadeWhenDoneProp);
            EditorGUILayout.PropertyField(waitForClickProp);
            EditorGUILayout.PropertyField(stopVoiceoverProp);
            EditorGUILayout.PropertyField(setSayDialogProp);
            EditorGUILayout.PropertyField(waitForVOProp);
            
            if (showPortraits && t.Portrait != null)
            {
                Texture2D characterTexture = t.Portrait.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                {
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }    
}