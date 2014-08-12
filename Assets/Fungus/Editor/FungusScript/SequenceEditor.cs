using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(Sequence))]
	public class SequenceEditor : Editor 
	{
		FungusCommand activeCommand;

		static public Sequence SequenceField(GUIContent label, FungusScript fungusScript, Sequence sequence)
		{
			if (fungusScript == null)
			{
				return null;
			}
			
			Sequence result = sequence;
			
			// Build dictionary of child sequences
			List<GUIContent> sequenceNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			sequenceNames.Add(new GUIContent("<None>"));
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
				result = null; // Option 'None'
			}
			else
			{
				result = sequences[selectedIndex - 1];
			}
			
			return result;
		}

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		/*
		public void OnInspectorUpdate()
		{
			Repaint();
		}
		*/

		public override void OnInspectorGUI()
		{
			Sequence t = target as Sequence;

			FungusCommand[] commands = t.GetComponents<FungusCommand>();

			/*
			if (Application.isPlaying)
			{
				foreach (FungusCommand command in commands)
				{
					if (command == FungusCommandEditor.selectedCommand)
					{
						activeCommand = command;
						Debug.Log("Found it");
					}
				}
			}
			*/

			foreach (FungusCommand command in commands)
			{
				bool showCommandInspector = false;
				if (command == activeCommand ||
				    command.IsExecuting())
				{
					showCommandInspector = true;
				}

				if (GUILayout.Button(command.GetCommandTitle()))
				{
					if (activeCommand == command)
					{
						activeCommand = null;
					}
					else
					{
						activeCommand = command;
					}
				}

				if (showCommandInspector)
				{
					Editor commandEditor = Editor.CreateEditor(command);
					commandEditor.OnInspectorGUI();
				}
			}
		}
	}

}