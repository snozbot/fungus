using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(Sequence))]
	public class SequenceEditor : Editor 
	{
		SerializedProperty sequenceNameProp;
		SerializedProperty descriptionProp;

		void OnEnable()
		{
			descriptionProp = serializedObject.FindProperty("description");
		}

		public void DrawSequenceGUI(FungusScript fungusScript)
		{
			if (fungusScript.selectedSequence == null)
			{
				return;
			}

			serializedObject.Update();
			
			Sequence sequence = fungusScript.selectedSequence;

			EditorGUI.BeginChangeCheck();
			string sequenceName = EditorGUILayout.TextField(new GUIContent("Name", "Name of sequence object"), sequence.gameObject.name);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(sequence.gameObject, "Set Sequence Name");
				sequence.gameObject.name = sequenceName;
			}

			EditorGUILayout.PropertyField(descriptionProp);

			EditorGUILayout.Separator();

			UpdateIndentLevels(sequence);

			ReorderableListGUI.Title("Command Sequence");
			SerializedProperty commandListProperty = serializedObject.FindProperty("commandList");
			CommandListAdaptor adaptor = new CommandListAdaptor(commandListProperty, 0);
			ReorderableListControl.DrawControlFromState(adaptor, null, 0);

			if (Application.isPlaying)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}
			
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.BeginHorizontal();

			// Build list of categories
			List<string> categories = new List<string>();
			List<System.Type> subTypes = EditorExtensions.FindDerivedTypes(typeof(Command)).ToList();
			foreach(System.Type type in subTypes)
			{
				object[] attributes = type.GetCustomAttributes(false);
				foreach (object obj in attributes)
				{
					CommandInfoAttribute categoryAttr = obj as CommandInfoAttribute;
					if (categoryAttr != null)
					{
						if (!categories.Contains(categoryAttr.Category))
						{
							categories.Add(categoryAttr.Category);
						}
					}
				}
			}
			categories.Sort();

			GUILayout.Label("New Command");
			GUILayout.FlexibleSpace();

			// We should probably use SerializedProperty for the category & command index but there's no real benefit to doing so
			int selectedCategoryIndex = EditorGUILayout.Popup(fungusScript.selectedCommandCategoryIndex, categories.ToArray());
			
			List<string> commandNames = new List<string>();
			List<System.Type> commandTypes = new List<System.Type>();
			
			string categoryName = categories[selectedCategoryIndex];
			foreach (System.Type type in subTypes)
			{
				CommandInfoAttribute commandInfoAttr = CommandEditor.GetCommandInfo(type);
				if (commandInfoAttr == null)
				{
					continue;
				}

				if (categoryName == commandInfoAttr.Category)
				{
					commandNames.Add(commandInfoAttr.CommandName);
					commandTypes.Add(type);
				}
			}
			
			int selectedCommandIndex = EditorGUILayout.Popup(fungusScript.selectedAddCommandIndex, commandNames.ToArray());
			if (selectedCategoryIndex != fungusScript.selectedCommandCategoryIndex)
			{
				// Default to first item in list if category has changed
				selectedCommandIndex = 0;
			}

			EditorGUILayout.EndHorizontal();

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(fungusScript, "Select Command");
				fungusScript.selectedCommandCategoryIndex = selectedCategoryIndex;
				fungusScript.selectedAddCommandIndex = selectedCommandIndex;
			}
			
			if (selectedCommandIndex >= commandTypes.Count)
			{
				return;
			}
			
			System.Type selectedType = commandTypes[selectedCommandIndex];
			if (fungusScript.selectedSequence == null ||
			    selectedType == null)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}

			fungusScript.selectedAddCommandType = selectedType;

			EditorGUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();
		
			if (fungusScript.copyCommand != null)
			{
				if (GUILayout.Button("Paste"))
				{
					fungusScript.selectedCommand = CommandEditor.PasteCommand(fungusScript.copyCommand, fungusScript.selectedSequence);
				}
			}

			EditorGUILayout.EndHorizontal();

			CommandInfoAttribute infoAttr = CommandEditor.GetCommandInfo(selectedType);
			if (infoAttr != null)
			{
				GUIStyle labelStyle = new GUIStyle(EditorStyles.miniLabel);
				labelStyle.wordWrap = true;
				EditorGUILayout.HelpBox(infoAttr.HelpText, MessageType.Info);
			}

			serializedObject.ApplyModifiedProperties();
		}

		void UpdateIndentLevels(Sequence sequence)
		{
			int indentLevel = 0;
			foreach(Command command in sequence.commandList)
			{
				indentLevel += command.GetPreIndent();
				command.indentLevel = Math.Max(indentLevel, 0);
				indentLevel += command.GetPostIndent();
			}
		}

		static public void SequenceField(SerializedProperty property, GUIContent label, GUIContent nullLabel, FungusScript fungusScript)
		{
			if (fungusScript == null)
			{
				return;
			}

			Sequence sequence = property.objectReferenceValue as Sequence;
		
			// Build dictionary of child sequences
			List<GUIContent> sequenceNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			sequenceNames.Add(nullLabel);
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
			for (int i = 0; i < sequences.Length; ++i)
			{
				sequenceNames.Add(new GUIContent(sequences[i].name));
				
				if (sequence == sequences[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label, selectedIndex, sequenceNames.ToArray());
			if (selectedIndex == 0)
			{
				sequence = null; // Option 'None'
			}
			else
			{
				sequence = sequences[selectedIndex - 1];
			}
			
			property.objectReferenceValue = sequence;
		}

		static public Sequence SequenceField(Rect position, GUIContent nullLabel, FungusScript fungusScript, Sequence sequence)
		{
			if (fungusScript == null)
			{
				return null;
			}
			
			Sequence result = sequence;
			
			// Build dictionary of child sequences
			List<GUIContent> sequenceNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			sequenceNames.Add(nullLabel);
			Sequence[] sequences = fungusScript.GetComponentsInChildren<Sequence>();
			for (int i = 0; i < sequences.Length; ++i)
			{
				sequenceNames.Add(new GUIContent(sequences[i].name));
				
				if (sequence == sequences[i])
				{
					selectedIndex = i + 1;
				}
			}
			
			selectedIndex = EditorGUI.Popup(position, selectedIndex, sequenceNames.ToArray());
			if (selectedIndex == 0)
			{
				result = null; // Option 'None'
			}
			else
			{
				result = sequences[selectedIndex - 1];
			}
			
			return result;
		}
	}

}