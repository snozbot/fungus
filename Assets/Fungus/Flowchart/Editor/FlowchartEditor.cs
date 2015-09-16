using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Fungus
{
	[CustomEditor (typeof(Flowchart))]
	public class FlowchartEditor : Editor 
	{
		protected class AddVariableInfo
		{
			public Flowchart flowchart;
			public System.Type variableType;
		}

		protected SerializedProperty descriptionProp;
		protected SerializedProperty colorCommandsProp;
		protected SerializedProperty hideComponentsProp;
		protected SerializedProperty stepPauseProp;
		protected SerializedProperty saveSelectionProp;
		protected SerializedProperty localizationIdProp;
		protected SerializedProperty variablesProp;
		protected SerializedProperty hideCommandsProp;

		protected Texture2D addTexture;
				
		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			descriptionProp = serializedObject.FindProperty("description");
			colorCommandsProp = serializedObject.FindProperty("colorCommands");
			hideComponentsProp = serializedObject.FindProperty("hideComponents");
			stepPauseProp = serializedObject.FindProperty("stepPause");
			saveSelectionProp = serializedObject.FindProperty("saveSelection");
			localizationIdProp = serializedObject.FindProperty("localizationId");
			variablesProp = serializedObject.FindProperty("variables");
			hideCommandsProp = serializedObject.FindProperty("hideCommands");

			addTexture = Resources.Load("Icons/add_small") as Texture2D;
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			Flowchart flowchart = target as Flowchart;

			flowchart.UpdateHideFlags();

			EditorGUILayout.PropertyField(descriptionProp);
			EditorGUILayout.PropertyField(colorCommandsProp);
			EditorGUILayout.PropertyField(hideComponentsProp);
			EditorGUILayout.PropertyField(stepPauseProp);
			EditorGUILayout.PropertyField(saveSelectionProp);
			EditorGUILayout.PropertyField(localizationIdProp);

			// Show list of commands to hide in Add Command menu
			ReorderableListGUI.Title(new GUIContent(hideCommandsProp.displayName, hideCommandsProp.tooltip));
			ReorderableListGUI.ListField(hideCommandsProp);

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Flowchart Window"))
			{
				EditorWindow.GetWindow(typeof(FlowchartWindow), false, "Flowchart");
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			serializedObject.ApplyModifiedProperties();
		}

		public virtual void DrawVariablesGUI()
		{
			serializedObject.Update();

			Flowchart t = target as Flowchart;

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

				// Draw disclosure triangle
				Rect lastRect = GUILayoutUtility.GetLastRect();
				lastRect.x += 5;
				lastRect.y += 5;
				EditorGUI.Foldout(lastRect, false, "");
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

				// Draw disclosure triangle
				Rect lastRect = buttonRect;
				lastRect.x += 5;
				lastRect.y += 5;
				EditorGUI.Foldout(lastRect, true, "");

				Rect plusRect = listRect;
				plusRect.x += plusRect.width - plusWidth;
				plusRect.y -= plusHeight - 1;
				plusRect.width = plusWidth;
				plusRect.height = plusHeight;

				if (!Application.isPlaying && 
				    GUI.Button(plusRect, addTexture))
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
						addVariableInfo.flowchart = t;
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
						info.flowchart = t;
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

			Flowchart flowchart = addVariableInfo.flowchart;
			System.Type variableType = addVariableInfo.variableType;

			Undo.RecordObject(flowchart, "Add Variable");
			Variable newVariable = flowchart.gameObject.AddComponent(variableType) as Variable;
			newVariable.key = flowchart.GetUniqueVariableKey("");
			flowchart.variables.Add(newVariable);
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

		/**
		 * When modifying custom editor code you can occasionally end up with orphaned editor instances.
		 * When this happens, you'll get a null exception error every time the scene serializes / deserialized.
		 * Once this situation occurs, the only way to fix it is to restart the Unity editor.
		 * As a workaround, this function detects if this editor is an orphan and deletes it. 
		 */
		protected virtual bool NullTargetCheck()
		{
			try
			{
				// The serializedObject accessor creates a new SerializedObject if needed.
				// However, this will fail with a null exception if the target object no longer exists.
				#pragma warning disable 0219
				SerializedObject so = serializedObject;
			}
			catch (System.NullReferenceException)
			{
				DestroyImmediate(this);
				return true;
			}
			
			return false;
		}
	}

}