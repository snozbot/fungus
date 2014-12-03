using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Fungus
{
	/**
	 * Temp hidden object which lets us use the entire inspector window to inspect
	 * the sequence command list.
	 */
	public class SequenceInspector : ScriptableObject 
	{
		public Sequence sequence;
	}

	/**
	 * Custom editor for the temp hidden object.
	 */
	[CustomEditor (typeof(SequenceInspector), true)]
	public class SequenceInspectorEditor : Editor
	{
		public override void OnInspectorGUI () 
		{
			SequenceInspector sequenceInspector = target as SequenceInspector;
			Sequence sequence = sequenceInspector.sequence;

			if (sequence == null)
			{
				return;
			}

			FungusScript fungusScript = sequence.GetFungusScript();

			SequenceEditor sequenceEditor = Editor.CreateEditor(sequence) as SequenceEditor;
			sequenceEditor.DrawSequenceGUI(fungusScript);
			DestroyImmediate(sequenceEditor);

			Command inspectCommand = null;
			if (fungusScript.selectedCommands.Count == 1)
			{
				inspectCommand = fungusScript.selectedCommands[0];
			}

			if (Application.isPlaying &&
			    inspectCommand != null &&
			    inspectCommand.parentSequence != sequence)
			{
				Repaint();
				return;
			}

			if (inspectCommand != null)
			{
				CommandEditor commandEditor = Editor.CreateEditor(inspectCommand) as CommandEditor;
				commandEditor.DrawCommandInspectorGUI();
				DestroyImmediate(commandEditor);
			}

			Repaint();
		}
	}

}
