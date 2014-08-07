using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(FungusVariable), true)]
	public class FungusVariableEditor : FungusCommandEditor
	{
		public override void OnInspectorGUI() 
		{
			FungusVariable t = target as FungusVariable;

			EditorGUI.BeginChangeCheck();

			string key = EditorGUILayout.TextField(new GUIContent("Key", "Name to use for this variable"), t.key);
			VariableScope scope = (VariableScope)EditorGUILayout.EnumPopup(new GUIContent("Scope", "Local or global access to variable value"), t.scope);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(t, "Set Variable");
				t.key = key;
				t.scope = scope;
			}
		}
	}

}