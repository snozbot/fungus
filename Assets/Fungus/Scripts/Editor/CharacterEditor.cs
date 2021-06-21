// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Character))]
    public class CharacterEditor : Editor
    {
        protected SerializedProperty nameTextProp;
        protected SerializedProperty nameColorProp;
        protected SerializedProperty soundEffectProp;
        protected SerializedProperty portraitsProp;
        protected SerializedProperty portraitsFaceProp;
        protected SerializedProperty descriptionProp;
        protected SerializedProperty setSayDialogProp;
        protected SerializedProperty effectAudioSourceProp;
        protected SerializedProperty voiceAudioSourceProp;
        protected bool toggleLock = false;
        protected bool defaultPortrait = false;
        protected int counter = 0;

        protected virtual void OnEnable()
        {
            nameTextProp = serializedObject.FindProperty ("nameText");
            nameColorProp = serializedObject.FindProperty ("nameColor");
            soundEffectProp = serializedObject.FindProperty ("soundEffect");
            portraitsProp = serializedObject.FindProperty ("portraits");
            portraitsFaceProp = serializedObject.FindProperty ("portraitsFace");
            descriptionProp = serializedObject.FindProperty ("description");
            setSayDialogProp = serializedObject.FindProperty("setSayDialog");
            effectAudioSourceProp = serializedObject.FindProperty("effectAudioSource");
            voiceAudioSourceProp = serializedObject.FindProperty("voiceAudioSource");
        }

        public override void OnInspectorGUI() 
        {
            serializedObject.Update();

            Character t = target as Character;
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(nameTextProp, new GUIContent("Name Text", "Name of the character display in the dialog"));
            EditorGUILayout.PropertyField(nameColorProp, new GUIContent("Name Color", "Color of name text display in the dialog"));
            EditorGUILayout.PropertyField(soundEffectProp, new GUIContent("Sound Effect", "Sound to play when the character is talking. Overrides the setting in the Dialog."));
            EditorGUILayout.PropertyField(effectAudioSourceProp);
            EditorGUILayout.PropertyField(voiceAudioSourceProp);
            EditorGUILayout.PropertyField(setSayDialogProp);
            EditorGUILayout.PropertyField(descriptionProp, new GUIContent("Description", "Notes about this story character (personality, attibutes, etc.)"));

            if (t.Portraits != null &&
                t.Portraits.Count > 0)
            {
                if (!defaultPortrait)
                    t.ProfileSprite = t.Portraits[0];
            }
            else
            {
                t.ProfileSprite = null;
            }
            
            if (t.ProfileSprite != null)
            {
                Texture2D characterTexture = t.ProfileSprite.texture;
                float aspect = (float)characterTexture.width / (float)characterTexture.height;
                Rect previewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(100), GUILayout.ExpandWidth(true));
                if (characterTexture != null)
                    GUI.DrawTexture(previewRect,characterTexture,ScaleMode.ScaleToFit,true,aspect);
            }
            GUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();

            var myCustomStyle = new GUIStyle(GUI.skin.GetStyle("label"));
            myCustomStyle.wordWrap = true;
            myCustomStyle.richText = true;
            myCustomStyle.normal.textColor = Color.yellow;
            myCustomStyle.alignment = TextAnchor.MiddleCenter;

            if (GUILayout.Button("PREV", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (t.Portraits != null && t.Portraits.Count != 0)
                {
                    if (!defaultPortrait)
                    {
                        defaultPortrait = true;
                    }
                    if (counter != 0)
                    {
                        counter--;
                        if (t.Portraits[counter] != null)
                            t.ProfileSprite = t.Portraits[counter];
                    }
                    if (counter == 0)
                    {
                        t.ProfileSprite = t.Portraits[0];
                    }
                }
                else
                {
                    defaultPortrait = false;
                }
            }

            if (GUILayout.Button("NEXT", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (t.Portraits != null && t.Portraits.Count != 0)
                {
                    if (!defaultPortrait)
                    {
                        defaultPortrait = true;
                    }
                    if (counter != t.Portraits.Count - 1)
                    {
                        counter++;
                        if (t.Portraits[counter] != null)
                            t.ProfileSprite = t.Portraits[counter];
                    }
                }
                else
                {
                    defaultPortrait = false;
                }
            }

            if (GUILayout.Button("DEL", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (t.Portraits != null && t.Portraits.Count > 0)
                {
                    if (EditorUtility.DisplayDialog("Delete Character's Portrait", "Are you sure you want to delete this portrait!?", "DELETE", "CANCEL"))
                    {
                        t.Portraits.RemoveAt(counter);
                        t.Portraits.TrimExcess();
                        t.ProfileSprite = t.Portraits[counter];
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (GUILayout.Button("CLEAR", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (t.Portraits != null && t.Portraits.Count > 0)
                {
                    if (EditorUtility.DisplayDialog("REMOVE Character Portraits", "Are you sure you want to remove all portraits for this character!?", "REMOVE ALL", "CANCEL"))
                    {
                        t.Portraits.Clear();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            //Lock the Character inspector so selecting multiple files in assets folder will be easier without losing focus
            if (GUILayout.Button("LOCK", GUILayout.Width(50), GUILayout.Height(30)))
            {
                if (!toggleLock)
                {
                    toggleLock = true;
                    ActiveEditorTracker.sharedTracker.isLocked = true;
                }
                else
                {
                    toggleLock = false;
                    ActiveEditorTracker.sharedTracker.isLocked = false;
                }
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (t.ProfileSprite != null && t.Portraits.Count > 0)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.FlexibleSpace();
                GUILayout.Label("NO : " + counter + " : " + t.Portraits[counter].name);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            if (toggleLock)
            {
                GUILayout.BeginHorizontal("box");
                GUILayout.FlexibleSpace();
                GUILayout.Label("<b><size=15>INSPECTOR IS LOCKED!</size></b>\nPress Lock button to unlock", myCustomStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }

            //Drag n drop area;
            EditorGUILayout.Space();
            DropAreaGUI();

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(portraitsProp, new GUIContent("Portraits", "Character image sprites to display in the dialog"), true);

            EditorGUILayout.HelpBox("All portrait images should use the exact same resolution to avoid positioning and tiling issues.", MessageType.Info);

            EditorGUILayout.Separator();

            string[] facingArrows = new string[]
            {
                "FRONT",
                "<--",
                "-->",
            };
            portraitsFaceProp.enumValueIndex = EditorGUILayout.Popup("Portraits Face", (int)portraitsFaceProp.enumValueIndex, facingArrows);

            EditorGUILayout.Separator();

            if(EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(t);

            serializedObject.ApplyModifiedProperties();
        }

        public void DropAreaGUI()
        {
            Character t = target as Character;
            Event evt = Event.current;

            Rect drop_area = GUILayoutUtility.GetRect(0.0f, 60.0f, GUILayout.ExpandWidth(true));
            var centeredStyle = GUI.skin.GetStyle("Box");
            centeredStyle.alignment = TextAnchor.MiddleCenter;
            centeredStyle.fontStyle = FontStyle.Bold;
            centeredStyle.richText = true;

            GUI.Box(drop_area, "<size=24><color=green>DRAG & DROP</color></size>\n<color=red>CHARACTER SPRITES HERE</color>");
            switch (evt.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if (!drop_area.Contains(evt.mousePosition))
                        return;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();
                        var path = DragAndDrop.paths;
                        for (int i = 0; i < DragAndDrop.objectReferences.Length; i++)
                        {
                            var afterLoadedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path[i]);

                            // No need to check for types, if it's not a Sprite it will be nulled by default
                            if (afterLoadedSprite != null)
                                t.Portraits.Add(afterLoadedSprite);
                        }
                        ActiveEditorTracker.sharedTracker.ForceRebuild();
                    }
                    break;
            }
        }

    }
}