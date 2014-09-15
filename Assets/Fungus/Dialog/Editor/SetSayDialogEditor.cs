using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(SetSayDialog))]
	public class SetSayDialogEditor : CommandEditor
	{
		SerializedProperty sayDialogProp;
		
		void OnEnable()
		{
			sayDialogProp = serializedObject.FindProperty("sayDialog");
		}

		public override void DrawCommandGUI() 
		{
			serializedObject.Update();

			CommandEditor.ObjectField<SayDialog>(sayDialogProp,
			                                           new GUIContent("Say Dialog", "Dialog to use when displaying Say command story text"), 
			                                           new GUIContent("<None>"),
			                                           SayDialog.activeDialogs);

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}