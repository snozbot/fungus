using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;

namespace Fungus
{
	[CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
	{
		protected SerializedProperty startSequenceProp;
		protected SerializedProperty executeOnStartProp;
		protected SerializedProperty settingsProp;
		protected SerializedProperty variablesProp;

		protected virtual void OnEnable()
		{
			startSequenceProp = serializedObject.FindProperty("startSequence");
			executeOnStartProp = serializedObject.FindProperty("executeOnStart");
			settingsProp = serializedObject.FindProperty("settings");
			variablesProp = serializedObject.FindProperty("variables");
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			FungusScript fungusScript = target as FungusScript;

			fungusScript.UpdateHideFlags();

			SequenceEditor.SequenceField(startSequenceProp, 
			                             new GUIContent("Start Sequence", "First sequence to execute when the Fungus Script executes"), 
										 new GUIContent("<None>"),
			                             fungusScript);

			if (fungusScript.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			EditorGUILayout.PropertyField(executeOnStartProp, new GUIContent("Execute On Start", "Execute this Fungus Script when the scene starts playing"));

			EditorGUILayout.PropertyField(settingsProp, new GUIContent("Settings", "Configution options for the Fungus Script"), true);

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Editor"))
			{
				EditorWindow.GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawVariablesGUI()
		{
			serializedObject.Update();

			FungusScript t = target as FungusScript;

			if (!t.variablesExpanded)
			{
				if (GUILayout.Button ("Variables", GUILayout.Height(24)))
				{
					t.variablesExpanded = true;
				}
			}
			else
			{
				Rect listRect = new Rect();

				if (t.variables.Count > 0)
				{
					ReorderableListGUI.Title("Variables");
					VariableListAdaptor adaptor = new VariableListAdaptor(variablesProp, 0);

					ReorderableListFlags flags = ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton;
					if (Application.isPlaying)
					{
						flags |= ReorderableListFlags.HideRemoveButtons;
					}

					ReorderableListControl.DrawControlFromState(adaptor, null, flags);
					listRect = GUILayoutUtility.GetLastRect();
				}
				else
				{
					GUILayoutUtility.GetRect(300, 24);
					listRect = GUILayoutUtility.GetLastRect();
					listRect.y += 20;
				}

				float plusWidth = 32;
				float plusHeight = 24;

				Rect buttonRect = listRect;
				float buttonHeight = 24;
				buttonRect.x = 4;
				buttonRect.y -= buttonHeight - 1;
				buttonRect.height = buttonHeight;
				if (!Application.isPlaying)
				{
					buttonRect.width -= 30;
				}

				if (GUI.Button (buttonRect, "Variables"))
				{
					t.variablesExpanded = false;
				}

				Rect plusRect = listRect;
				plusRect.x += plusRect.width - plusWidth;
				plusRect.y -= plusHeight - 1;
				plusRect.width = plusWidth;
				plusRect.height = plusHeight;

				if (!Application.isPlaying && GUI.Button(plusRect, FungusEditorResources.texAddButton))
				{
					GenericMenu menu = new GenericMenu ();
					
					menu.AddItem(new GUIContent ("Boolean"), false, AddVariable<BooleanVariable>, t);
					menu.AddItem (new GUIContent ("Integer"), false, AddVariable<IntegerVariable>, t);
					menu.AddItem (new GUIContent ("Float"), false, AddVariable<FloatVariable>, t);
					menu.AddItem (new GUIContent ("String"), false, AddVariable<StringVariable>, t);

					menu.ShowAsContext ();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}
		
		protected virtual void AddVariable<T>(object obj) where T : Variable
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			Undo.RecordObject(fungusScript, "Add Variable");
			T newVariable = fungusScript.gameObject.AddComponent<T>();
			newVariable.key = fungusScript.GetUniqueVariableKey("");
			fungusScript.variables.Add(newVariable);
		}
	}
	
}