using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(SetChooseDialog))]
	public class SetChooseDialogEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			SetChooseDialog t = target as SetChooseDialog;

			EditorGUI.BeginChangeCheck();

			ChooseDialog dialog = FungusCommandEditor.ObjectField<ChooseDialog>(new GUIContent("Choose Dialog", "Dialog to use when displaying options with the Choose command."), 
			                                                              	 	new GUIContent("<None>"),
			                                                              	 	t.dialog,
			                                                              	 	ChooseDialog.activeDialogs);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Choose Dialog");
				t.dialog = dialog;
			}			
		}
	}
	
}