using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
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

		static public FungusVariable VariableField(GUIContent label, FungusScript fungusScript, FungusVariable variable, Func<FungusVariable, bool> filter = null)
		{
			List<string> variableKeys = new List<string>();
			List<FungusVariable> variableObjects = new List<FungusVariable>();
			
			variableKeys.Add("<None>");
			variableObjects.Add(null);
			
			FungusVariable[] variables = fungusScript.GetComponents<FungusVariable>();
			int index = 0;
			int selectedIndex = 0;
			foreach (FungusVariable v in variables)
			{
				if (filter != null)
				{
					if (!filter(v))
					{
						continue;
					}
				}
				
				variableKeys.Add(v.key);
				variableObjects.Add(v);
				
				index++;
				
				if (v == variable)
				{
					selectedIndex = index;
				}
			}
			
			selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
			
			return variableObjects[selectedIndex];
		}
	}

}