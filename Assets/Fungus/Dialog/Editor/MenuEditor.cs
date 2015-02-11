using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(Fungus.Menu))]
	public class MenuEditor : CommandEditor 
	{
		protected SerializedProperty textProp;
		protected SerializedProperty targetSequenceProp;
		protected SerializedProperty hideIfVisitedProp;

		protected virtual void OnEnable()
		{
			textProp = serializedObject.FindProperty("text");
			targetSequenceProp = serializedObject.FindProperty("targetSequence");
			hideIfVisitedProp = serializedObject.FindProperty("hideIfVisited");
		}
		
		public override void DrawCommandGUI()
		{
			FungusScript fungusScript = FungusScriptWindow.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}
			
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(textProp);
			
			SequenceEditor.SequenceField(targetSequenceProp,
			                             new GUIContent("Target Sequence", "Sequence to call when option is selected"), 
			                             new GUIContent("<None>"), 
			                             fungusScript);
			
			EditorGUILayout.PropertyField(hideIfVisitedProp);

			serializedObject.ApplyModifiedProperties();
		}
	}
	
}
