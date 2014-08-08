using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;
using Rotorz.ReorderableList;
using System.Linq;

namespace Fungus.Script
{
	[CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
	{
		SerializedProperty variablesProperty;

		void OnEnable() 
		{
			variablesProperty = serializedObject.FindProperty("variables");
		}

		public override void OnInspectorGUI() 
		{
			serializedObject.Update();

			FungusScript t = target as FungusScript;
		
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Fungus Editor"))
			{
				EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor");
			}
			
			if (GUILayout.Button("New Sequence"))
			{
				GameObject go = new GameObject("Sequence");
				go.transform.parent = t.transform;
				Sequence s = go.AddComponent<Sequence>();
				FungusEditorWindow fungusEditorWindow = EditorWindow.GetWindow(typeof(FungusEditorWindow), false, "Fungus Editor") as FungusEditorWindow;
				s.nodeRect.x = fungusEditorWindow.scrollPos.x;
				s.nodeRect.y = fungusEditorWindow.scrollPos.y;
				Undo.RegisterCreatedObjectUndo(go, "Sequence");
				Selection.activeGameObject = go;
			}

			if (GUILayout.Button("Add Variable"))
			{
				GenericMenu menu = new GenericMenu ();
				
				menu.AddItem(new GUIContent ("Boolean"), false, AddBooleanVariable, t);
				menu.AddItem (new GUIContent ("Integer"), false, AddIntegerVariable, t);
				menu.AddItem (new GUIContent ("Float"), false, AddFloatVariable, t);
				menu.AddItem (new GUIContent ("String"), false, AddStringVariable, t);

				menu.ShowAsContext ();
			}
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUIContent stepTimeLabel = new GUIContent("Step Time", "Minimum time to execute each step");
			t.stepTime = EditorGUILayout.FloatField(stepTimeLabel, t.stepTime);

			GUIContent startSequenceLabel = new GUIContent("Start Sequence", "Sequence to be executed when controller starts.");
			t.startSequence = SequenceEditor.SequenceField(startSequenceLabel, t, t.startSequence);

			GUIContent startAutomaticallyLabel = new GUIContent("Start Automatically", "Start this Fungus Script when the scene starts.");
			t.startAutomatically = EditorGUILayout.Toggle(startAutomaticallyLabel, t.startAutomatically);

			if (t.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			ReorderableListGUI.Title("Variables");
			ReorderableListGUI.ListField(variablesProperty);

			serializedObject.ApplyModifiedProperties();
		}

		void AddBooleanVariable(object obj)
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			FungusVariable variable = fungusScript.gameObject.AddComponent<BooleanVariable>();
			variable.key = MakeUniqueKey(fungusScript);
		}

		void AddIntegerVariable(object obj)
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			FungusVariable variable = fungusScript.gameObject.AddComponent<IntegerVariable>();
			variable.key = MakeUniqueKey(fungusScript);
		}

		void AddFloatVariable(object obj)
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			FungusVariable variable = fungusScript.gameObject.AddComponent<FloatVariable>();
			variable.key = MakeUniqueKey(fungusScript);
		}

		void AddStringVariable(object obj)
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			FungusVariable variable = fungusScript.gameObject.AddComponent<StringVariable>();
			variable.key = MakeUniqueKey(fungusScript);
		}

		string MakeUniqueKey(FungusScript fungusScript)
		{
			FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();

			int index = 0;
			while (true)
			{
				string key = "Var" + index;

				bool found = false;
				foreach(FungusVariable variable in variables)
				{
					if (variable.key == key)
					{
						found = true;
						index++;
					}
				}

				if (!found)
				{
					return key;
				}
			}
		}
	}

}