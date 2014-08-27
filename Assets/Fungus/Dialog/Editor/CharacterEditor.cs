using UnityEditor;
using UnityEngine;
using System.Collections;

namespace Fungus.Script
{

	[CustomEditor (typeof(Character))]
	public class CharacterEditor : Editor
	{
		Material spriteMaterial;

		void OnEnable()
		{
			Shader shader = Shader.Find("Sprites/Default");
			if (shader != null)
			{
				spriteMaterial = new Material(shader);
				spriteMaterial.hideFlags = HideFlags.DontSave;
			}
		}

		void OnDisable()
		{
			DestroyImmediate(spriteMaterial);
		}

		public override void OnInspectorGUI() 
		{
			Character t = target as Character;

			EditorGUI.BeginChangeCheck();

			string characterName = EditorGUILayout.TextField(new GUIContent("Name Text", "Name of the character display in the dialog"),
			                                                 t.characterName);

			Color characterColor = EditorGUILayout.ColorField(new GUIContent("Text Color", "Color of name text display in the dialog"),
			                                                  t.characterColor);

			Sprite characterImage = EditorGUILayout.ObjectField(new GUIContent("Image", "Character image sprite to display in the dialog"),
			                                                    t.characterImage,
			                                                    typeof(Sprite),
			                                                    true) as Sprite;

			Dialog.DialogSide dialogSide = (Dialog.DialogSide)EditorGUILayout.EnumPopup(new GUIContent("Image Side", "Which side to display the image in the dialog"),
			                                                                            t.dialogSide);

			EditorGUILayout.Separator();

			if (characterImage != null &&
			    spriteMaterial != null)
			{
				float aspect = (float)characterImage.texture.width / (float)characterImage.texture.height;

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				Rect imagePreviewRect = GUILayoutUtility.GetAspectRect(aspect, GUILayout.Width(150), GUILayout.ExpandWidth(false));
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUI.DrawPreviewTexture(imagePreviewRect, 
				                             characterImage.texture,
				                             spriteMaterial);
			}

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Character");

				t.characterName = characterName;
				t.characterColor = characterColor;
				t.characterImage = characterImage;
				t.dialogSide = dialogSide;
			}			
		}
	}

}