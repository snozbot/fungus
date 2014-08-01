using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{
	
	[CustomEditor (typeof(Jump))]
	public class JumpEditor : FungusCommandEditor
	{
		public override void DrawCommandInspectorGUI() 
		{
			Jump t = target as Jump;
			
			EditorGUI.BeginChangeCheck();
			
			Sequence newSequence = SequenceEditor.SequenceField(new GUIContent("Sequence", "Sequence to jump to"), 
			                                                    t.GetFungusScript(), 
			                                                    t.targetSequence);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Jump command");
				t.targetSequence = newSequence;
			}
		}
	}
	
}