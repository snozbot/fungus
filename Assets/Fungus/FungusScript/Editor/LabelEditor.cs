using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fungus
{
	public class LabelEditor
	{

		static public void LabelField(SerializedProperty property, 
		                              GUIContent labelText, 
		                              Sequence sequence)
		{
			List<string> labelKeys = new List<string>();
			List<Label> labelObjects = new List<Label>();
			
			labelKeys.Add("<None>");
			labelObjects.Add(null);
			
			Label selectedLabel = property.objectReferenceValue as Label;

			int index = 0;
			int selectedIndex = 0;
			foreach (Command command in sequence.commandList)
			{
				Label label = command as Label;
				if (label == null)
				{
					continue;
				}

				labelKeys.Add(label.key);
				labelObjects.Add(label);
				
				index++;
				
				if (label == selectedLabel)
				{
					selectedIndex = index;
				}
			}

			selectedIndex = EditorGUILayout.Popup(labelText.text, selectedIndex, labelKeys.ToArray());

			property.objectReferenceValue = labelObjects[selectedIndex];
		}
	}
	
}