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
		protected SerializedProperty sequenceNameProp;
		protected SerializedProperty descriptionProp;

		public virtual void OnEnable()
		{
			descriptionProp = serializedObject.FindProperty("description");
		}

		public virtual void DrawSequenceGUI(FungusScript fungusScript)
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

			if (fungusScript.selectedCommand != null)
			{
				CommandInfoAttribute infoAttr = CommandEditor.GetCommandInfo(fungusScript.selectedCommand.GetType());
				if (infoAttr != null)
				{
					EditorGUILayout.HelpBox(infoAttr.HelpText, MessageType.Info);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		protected virtual void UpdateIndentLevels(Sequence sequence)
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