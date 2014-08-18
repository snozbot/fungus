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
			if (this != null && serializedObject != null)
			{
				variablesProperty = serializedObject.FindProperty("variables");
			}
		}
		
		public void OnInspectorUpdate()
		{
			Repaint();
		}
		
		public override void OnInspectorGUI() 
		{
			serializedObject.Update();
			
			FungusScript t = target as FungusScript;

			if (t != null)
			{
				// Hide the transform component if FungusScript is the only component on the game object
				Component[] components = t.GetComponents(typeof(Component));
				t.transform.hideFlags = (components.Length == 2) ? HideFlags.HideInInspector : HideFlags.None;
			}

			EditorGUI.BeginChangeCheck();

			float stepTime = EditorGUILayout.FloatField(new GUIContent("Step Time", "Minimum time to execute each step"), t.stepTime);

			Sequence startSequence = SequenceEditor.SequenceField(new GUIContent("Start Sequence", "Sequence to be executed when controller starts."), 
			                                                      new GUIContent("<None>"),
			                                                      t, 
			                                                      t.startSequence);
			if (t.startSequence == null)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: Please select a Start Sequence"), style);
			}

			bool startAutomatically = EditorGUILayout.Toggle(new GUIContent("Start Automatically", "Start this Fungus Script when the scene starts."), t.startAutomatically);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Fungus Script");
				t.stepTime = stepTime;
				t.startSequence = startSequence;
				t.startAutomatically = startAutomatically;
			}

			EditorGUILayout.Separator();

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Open Editor"))
			{
				EditorWindow.GetWindow(typeof(FungusScriptWindow), false, "Fungus Script");
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();
			GUI.backgroundColor = Color.yellow;
			GUILayout.Box("Sequence Editor", GUILayout.ExpandWidth(true));
			GUI.backgroundColor = Color.white;

			GUILayout.BeginHorizontal();

			if (t.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}

			if (GUILayout.Button(t.selectedSequence == null ? "Create Sequence" : "Create", 
			                     t.selectedSequence == null ?  EditorStyles.miniButton : EditorStyles.miniButtonLeft))
			{
				Sequence newSequence = t.CreateSequence(t.scrollPos);
				Undo.RegisterCreatedObjectUndo(newSequence, "New Sequence");
				t.selectedSequence = newSequence;
			}

			if (t.selectedSequence == null)
			{
				GUILayout.FlexibleSpace();
			}

			if (t.selectedSequence != null)
			{
				if (GUILayout.Button("Delete", EditorStyles.miniButtonMid))
				{
					Undo.DestroyObjectImmediate(t.selectedSequence.gameObject);
					t.selectedSequence = null;
				}
				if (GUILayout.Button("Duplicate", EditorStyles.miniButtonRight))
				{
					GameObject copy = GameObject.Instantiate(t.selectedSequence.gameObject) as GameObject;
					copy.transform.parent = t.transform;
					copy.transform.hideFlags = HideFlags.HideInHierarchy;
					copy.name = t.selectedSequence.name;
					
					Sequence sequenceCopy = copy.GetComponent<Sequence>();
					sequenceCopy.nodeRect.x += sequenceCopy.nodeRect.width + 10;
					
					Undo.RegisterCreatedObjectUndo(copy, "Duplicate Sequence");
					t.selectedSequence = sequenceCopy;
				}
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			if (t.selectedSequence != null)
			{
				DrawSequenceGUI(t.selectedSequence);

				if (!Application.isPlaying)
				{
					EditorGUILayout.Separator();
					DrawAddCommandGUI(t.selectedSequence);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		public void DrawSequenceGUI(Sequence sequence)
		{
			EditorGUI.BeginChangeCheck();

			string name = EditorGUILayout.TextField(new GUIContent("Sequence Name", "Name of sequence displayed in editor window"), sequence.name);
			EditorGUILayout.PrefixLabel(new GUIContent("Description", "Sequence description displayed in editor window"));
			GUIStyle descriptionStyle = new GUIStyle(EditorStyles.textArea);
			descriptionStyle.wordWrap = true;
			string desc = EditorGUILayout.TextArea(sequence.description, descriptionStyle);

			EditorGUILayout.Separator();

			if (name != sequence.name)
			{
				// The name is the gameobject name, so have to undo seperately
				Undo.RecordObject(sequence.gameObject, "Set Sequence Name");
				sequence.name = name;
			}

			if (desc != sequence.description)
			{
				Undo.RecordObject(sequence, "Set Sequence Description");
				sequence.description = desc;
			}

			EditorGUILayout.PrefixLabel("Commands");

			FungusCommand[] commands = sequence.GetComponents<FungusCommand>();
			int index = 1;
			foreach (FungusCommand command in commands)
			{
				FungusCommandEditor commandEditor = Editor.CreateEditor(command) as FungusCommandEditor;
				commandEditor.DrawInspectorGUI(index++);
			}
		}

		public void DrawAddCommandGUI(Sequence sequence)
		{
			FungusScript t = target as FungusScript;

			GUI.backgroundColor = Color.yellow;
			GUILayout.Box("Add Command", GUILayout.ExpandWidth(true));
			GUI.backgroundColor = Color.white;

			// Build list of categories
			List<System.Type> commandTypes = EditorExtensions.FindDerivedTypes(typeof(FungusCommand)).ToList();
			if (commandTypes.Count == 0)
			{
				return;
			}

			List<string> commandNames = new List<string>();
			foreach(System.Type type in commandTypes)
			{
				commandNames.Add(type.Name);
			}

			EditorGUI.BeginChangeCheck();

			int selectedCommandIndex = EditorGUILayout.Popup("Command", t.selectedCommandIndex, commandNames.ToArray());

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Select Command");
				t.selectedCommandIndex = selectedCommandIndex;
			}

			System.Type selectedType = commandTypes[selectedCommandIndex];
			if (selectedType == null)
			{
				return;
			}

			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(new GUIContent("Add Command", "Add the selected command to the sequence")))
			{
				Undo.AddComponent(sequence.gameObject, selectedType);
			}
			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();
							
			object[] attributes = selectedType.GetCustomAttributes(typeof(HelpTextAttribute), false);
			foreach (object obj in attributes)
			{
				HelpTextAttribute helpTextAttr = obj as HelpTextAttribute;
				if (helpTextAttr != null)
				{
					GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
					labelStyle.wordWrap = true;
					EditorGUILayout.HelpBox(helpTextAttr.HelpText, MessageType.Info);
					break;
				}
			}
		}

		public void DrawVariablesGUI()
		{
			serializedObject.Update();
			
			FungusScript t = target as FungusScript;
			
			ReorderableListGUI.Title("Variables");
			
			FungusVariableListAdaptor adaptor = new FungusVariableListAdaptor(variablesProperty, 0);
			ReorderableListControl.DrawControlFromState(adaptor, null, ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton);
			
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			
			if (!Application.isPlaying && GUILayout.Button("Add Variable"))
			{
				GenericMenu menu = new GenericMenu ();
				
				menu.AddItem(new GUIContent ("Boolean"), false, AddVariable<BooleanVariable>, t);
				menu.AddItem (new GUIContent ("Integer"), false, AddVariable<IntegerVariable>, t);
				menu.AddItem (new GUIContent ("Float"), false, AddVariable<FloatVariable>, t);
				menu.AddItem (new GUIContent ("String"), false, AddVariable<StringVariable>, t);
				
				menu.ShowAsContext ();
			}
			GUILayout.EndHorizontal();
			
			serializedObject.ApplyModifiedProperties();
		}
		
		void AddVariable<T>(object obj) where T : FungusVariable
		{
			FungusScript fungusScript = obj as FungusScript;
			if (fungusScript == null)
			{
				return;
			}
			
			Undo.RecordObject(fungusScript, "Add Variable");
			T variable = fungusScript.gameObject.AddComponent<T>();
			variable.key = MakeUniqueKey(fungusScript);
			fungusScript.variables.Add(variable);
		}

		string MakeUniqueKey(FungusScript fungusScript)
		{
			int index = 0;
			while (true)
			{
				string key = "Var" + index;
				
				bool found = false;
				foreach(FungusVariable variable in fungusScript.GetComponents<FungusVariable>())
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