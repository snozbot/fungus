using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus.Script
{

	[CustomEditor (typeof(SetSayDialog))]
	public class SetSayDialogEditor : FungusCommandEditor
	{
		public override void DrawCommandGUI() 
		{
			SetSayDialog t = target as SetSayDialog;

			EditorGUI.BeginChangeCheck();

			SayDialog dialog = FungusCommandEditor.ObjectField<SayDialog>(new GUIContent("Say Dialog", "Dialog to use when displaying Say command story text"), 
			                                                              new GUIContent("<None>"),
			                                                              t.dialog,
			                                                              SayDialog.activeDialogs);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Say Dialog");
				t.dialog = dialog;
			}			
		}
	}
	
}