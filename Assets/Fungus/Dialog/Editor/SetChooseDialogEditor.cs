using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(SetChooseDialog))]
	public class SetChooseDialogEditor : CommandEditor
	{
		SerializedProperty chooseDialogProp;

		void OnEnable()
		{
			chooseDialogProp = serializedObject.FindProperty("chooseDialog");
		}

		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			CommandEditor.ObjectField<ChooseDialog>(chooseDialogProp,
			                                              new GUIContent("Choose Dialog", "Dialog to use when displaying options with the Choose command."), 
			                                              new GUIContent("<None>"),
			                                              ChooseDialog.activeDialogs);
			serializedObject.ApplyModifiedProperties();
		}
	}
	
}