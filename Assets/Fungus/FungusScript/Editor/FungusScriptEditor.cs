using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;
using System.Reflection;

namespace Fungus
{
	[CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
	{
		protected class AddVariableInfo
		{
			public FungusScript fungusScript;
			public System.Type variableType;
		}

		protected SerializedProperty descriptionProp;
		protected SerializedProperty colorCommandsProp;
		protected SerializedProperty hideComponentsProp;
		protected SerializedProperty runSlowDurationProp;
		protected SerializedProperty variablesProp;

		protected virtual void OnEnable()
		{
			descriptionProp = serializedObject.FindProperty("description");
			colorCommandsProp = serializedObject.FindProperty("colorCommands");
			hideComponentsProp = serializedObject.FindProperty("hideComponents");
			runSlowDurationProp = serializedObject.FindProperty("runSlowDuration");
			variablesProp = serializedObject.FindProperty("variables");
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			FungusScript fungusScript = target as FungusScript;

			fungusScript.UpdateHideFlags();

			EditorGUILayout.PropertyField(descriptionProp);
			EditorGUILayout.PropertyField(colorCommandsProp);
			EditorGUILayout.PropertyField(hideComponentsProp);
			EditorGUILayout.PropertyField(runSlowDurationProp);

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

			if (t.variables.Count == 0)
			{
				t.variablesExpanded = true;
			}

			if (!t.variablesExpanded)
			{
				if (GUILayout.Button ("Variables (" + t.variables.Count + ")", GUILayout.Height(24)))
				{
					t.variablesExpanded = true;
				}
			}
			else
			{
				Rect listRect = new Rect();

				if (t.variables.Count > 0)
				{
					// Remove any null variables from the list
					// Can sometimes happen when upgrading to a new version of Fungus (if .meta GUID changes for a variable class)
					for (int i = t.variables.Count - 1; i >= 0; i--)
					{
						if (t.variables[i] == null)
						{
							t.variables.RemoveAt(i);
						}
					}

					ReorderableListGUI.Title("Variables");
					VariableListAdaptor adaptor = new VariableListAdaptor(variablesProp, 0);

					ReorderableListFlags flags = ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton;

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

				if (!Application.isPlaying && 
				    GUI.Button(plusRect, FungusEditorResources.texAddButton))
				{
					GenericMenu menu = new GenericMenu ();
					List<System.Type> types = FindAllDerivedTypes<Variable>();

					// Add variable types without a category
					foreach (System.Type type in types)
					{
						VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
						if (variableInfo == null ||
						    variableInfo.Category != "")
						{
							continue;
						}

						AddVariableInfo addVariableInfo = new AddVariableInfo();
						addVariableInfo.fungusScript = t;
						addVariableInfo.variableType = type;

						GUIContent typeName = new GUIContent(variableInfo.VariableType);

						menu.AddItem(typeName, false, AddVariable, addVariableInfo);
					}

					// Add types with a category
					foreach (System.Type type in types)
					{
						VariableInfoAttribute variableInfo = VariableEditor.GetVariableInfo(type);
						if (variableInfo == null ||
						    variableInfo.Category == "")
						{
							continue;
						}

						AddVariableInfo info = new AddVariableInfo();
						info.fungusScript = t;
						info.variableType = type;

						GUIContent typeName = new GUIContent(variableInfo.Category + "/" + variableInfo.VariableType);

						menu.AddItem(typeName, false, AddVariable, info);
					}

					menu.ShowAsContext ();
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void AddVariable(object obj)
		{
			AddVariableInfo addVariableInfo = obj as AddVariableInfo;
			if (addVariableInfo == null)
			{
				return;
			}

			FungusScript fungusScript = addVariableInfo.fungusScript;
			System.Type variableType = addVariableInfo.variableType;

			Undo.RecordObject(fungusScript, "Add Variable");
			Variable newVariable = fungusScript.gameObject.AddComponent(variableType) as Variable;
			newVariable.key = fungusScript.GetUniqueVariableKey("");
			fungusScript.variables.Add(newVariable);
		}

		public static List<System.Type> FindAllDerivedTypes<T>()
		{
			return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
		}
		
		public static List<System.Type> FindAllDerivedTypes<T>(Assembly assembly)
		{
			var derivedType = typeof(T);
			return assembly
				.GetTypes()
					.Where(t =>
					       t != derivedType &&
					       derivedType.IsAssignableFrom(t)
					       ).ToList();
			
		} 
	}
	
}