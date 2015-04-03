using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	
	[CustomEditor (typeof(Say))]
	public class SayEditor : CommandEditor
	{
		static public bool showTagHelp;
		public Texture2D blackTex;
		
		static public void DrawTagHelpLabel()
		{
			string tagsText = "";
			tagsText += "\n";
			tagsText += "\t-------- DEFAULT TAGS --------\n\n";
			tagsText += "" +
				    "\t{b} Bold Text {/b}\n" + 
				    "\t{i} Italic Text {/i}\n" +
					"\t{color=red} Color Text (color){/color}\n" +
					"\n" +
					"\t{s}, {s=60} Writing speed (chars per sec){/s}\n" +
					"\t{w}, {w=0.5} Wait (seconds)\n" +
					"\t{wi} Wait for input\n" +
					"\t{wc} Wait for input and clear\n" +
					"\t{wp}, {wp=0.5} Wait on punctuation (seconds){/wp}\n" +
					"\t{c} Clear\n" +
					"\t{x} Exit, advance to the next command without waiting for input\n" +
					"\n" +
					"\t{vpunch=0.5} Vertically punch screen (intensity)\n" +
					"\t{hpunch=0.5} Horizontally punch screen (intensity)\n" +
					"\t{shake=1} Shake screen (intensity)\n" +
					"\t{shiver=1} Shiver screen (intensity)\n" +
					"\t{flash=0.5} Flash screen (duration)\n" +
					"\n" +
					"\t{audio=AudioObjectName} Play Audio Once\n" +
					"\t{audioloop=AudioObjectName} Play Audio Loop\n" +
					"\t{audiopause=AudioObjectName} Pause Audio\n" +
					"\t{audiostop=AudioObjectName} Stop Audio\n" +
					"\n" +
					"\t{m} Broadcast message\n" +
					"\t{$VarName} Substitute variable";
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
						tagStartSymbol = parentTag.tagStartSymbol;
						tagEndSymbol = parentTag.tagEndSymbol;
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
							tagStartSymbol = childTag.tagStartSymbol;
							tagEndSymbol = childTag.tagEndSymbol;
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
		protected SerializedProperty fadeInProp;
		protected SerializedProperty fadeOutProp;
		protected SerializedProperty waitForClickProp;
		protected SerializedProperty setSayDialogProp;

		protected virtual void OnEnable()
		{
			characterProp = serializedObject.FindProperty("character");
			portraitProp = serializedObject.FindProperty("portrait");
			storyTextProp = serializedObject.FindProperty("storyText");
			descriptionProp = serializedObject.FindProperty("description");
			voiceOverClipProp = serializedObject.FindProperty("voiceOverClip");
			showAlwaysProp = serializedObject.FindProperty("showAlways");
			showCountProp = serializedObject.FindProperty("showCount");
			extendPreviousProp = serializedObject.FindProperty("extendPrevious");
			fadeInProp = serializedObject.FindProperty("fadeIn");
			fadeOutProp = serializedObject.FindProperty("fadeOut");
			waitForClickProp = serializedObject.FindProperty("waitForClick");
			setSayDialogProp = serializedObject.FindProperty("setSayDialog");
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
			                                     Character.activeCharacters);

			Say t = target as Say;

			// Only show portrait selection if...
			if (t.character != null &&              // Character is selected
			    t.character.portraits != null &&    // Character has a portraits field
			    t.character.portraits.Count > 0 )   // Selected Character has at least 1 portrait
			{
				showPortraits = true;    
			}

			if (showPortraits) 
			{
				CommandEditor.ObjectField<Sprite>(portraitProp, 
			                                	     new GUIContent("Portrait", "Portrait representing speaking character"), 
			                                    	 new GUIContent("<None>"),
			                                     	t.character.portraits);
			}
			else
			{
				if (!t.extendPrevious)
				{
					t.portrait = null;
				}
			}
			
			EditorGUILayout.PropertyField(storyTextProp);

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

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.PrefixLabel("Fade");
			t.fadeIn = GUILayout.Toggle(t.fadeIn, "In", leftButton, GUILayout.Width(60));
			t.fadeOut = GUILayout.Toggle(t.fadeOut, "Out", rightButton, GUILayout.Width(60));
			
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(waitForClickProp);
			EditorGUILayout.PropertyField(setSayDialogProp);

			if (showPortraits && t.portrait != null)
			{
				Texture2D characterTexture = t.portrait.texture;
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