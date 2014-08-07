using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.Script
{

	[CustomEditor (typeof(Sequence))]
	public class SequenceEditor : Editor 
	{
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

		static public FungusVariable VariableField(GUIContent label, FungusScript fungusScript, FungusVariable variable, Func<FungusVariable, bool> filter = null)
		{
			List<string> variableKeys = new List<string>();
			List<FungusVariable> variableObjects = new List<FungusVariable>();

			variableKeys.Add("<None>");
			variableObjects.Add(null);

			FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
			int index = 0;
			int selectedIndex = 0;
			foreach (FungusVariable v in variables)
			{
				if (filter != null)
				{
					if (!filter(v))
					{
						continue;
					}
				}

				variableKeys.Add(v.key);
				variableObjects.Add(v);

				index++;

				if (v == variable)
				{
					selectedIndex = index;
				}
			}

			selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());

			return variableObjects[selectedIndex];
		}
	}

}