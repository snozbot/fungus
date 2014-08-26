using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(SetDialog))]
	public class SetDialogEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			SetDialog t = target as SetDialog;

			EditorGUI.BeginChangeCheck();

			Dialog dialogController = FungusCommandEditor.ObjectField<Dialog>(new GUIContent("Active Dialog", "Dialog to use when displaying Say command story text"), 
			                                                                                      new GUIContent("<None>"),
			                                                                                      t.dialogController);						                                         
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Dialog");
				t.dialogController = dialogController;
			}			
		}
	}

}