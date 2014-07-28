using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fungus;

[CustomEditor (typeof(Sequence))]
public class SequenceEditor : Editor 
{

	static public Sequence SequenceField(string label, SequenceController sequenceController, Sequence sequence)
	{
		if (sequenceController == null)
		{
			return null;
		}
		
		Sequence result = sequence;
		
		// Build dictionary of child sequences
		List<string> sequenceNames = new List<string>();
		
		int selectedIndex = 0;
		sequenceNames.Add("None");
		Sequence[] sequences = sequenceController.GetComponentsInChildren<Sequence>();
		for (int i = 0; i < sequences.Length; ++i)
		{
			sequenceNames.Add(sequences[i].name);
			
			if (sequence == sequences[i])
			{
				selectedIndex = i + 1;
			}
		}
		
		selectedIndex = EditorGUILayout.Popup("Start Sequence", selectedIndex, sequenceNames.ToArray());
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