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
			SequenceInspector t = target as SequenceInspector;
			Sequence s = t.sequence;

			if (s == null)
			{
				return;
			}

			FungusScript fungusScript = s.GetFungusScript();

			SequenceEditor sequenceEditor = Editor.CreateEditor(s) as SequenceEditor;
			sequenceEditor.DrawCommandListGUI(fungusScript);
			DestroyImmediate(sequenceEditor);

			Command inspectCommand = null;

			if (Application.isPlaying &&
			    fungusScript.executingSequence != null)
			{
				inspectCommand = fungusScript.executingSequence.activeCommand;
			}
			else if (fungusScript.selectedCommands.Count == 1)
			{
				inspectCommand = fungusScript.selectedCommands[0];
			}

			if (Application.isPlaying &&
			    inspectCommand != null &&
			    inspectCommand.parentSequence != s)
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
