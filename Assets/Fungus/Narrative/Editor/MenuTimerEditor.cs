using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(MenuTimer))]
	public class MenuTimerEditor : CommandEditor 
	{
		protected SerializedProperty durationProp;
		protected SerializedProperty targetSequenceProp;

		protected virtual void OnEnable()
		{
			durationProp = serializedObject.FindProperty("duration");
			targetSequenceProp = serializedObject.FindProperty("targetSequence");
		}
		
		public override void DrawCommandGUI()
		{
			FungusScript fungusScript = FungusScriptWindow.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}
			
			serializedObject.Update();
			
			EditorGUILayout.PropertyField(durationProp);
			
			SequenceEditor.SequenceField(targetSequenceProp,
			                             new GUIContent("Target Sequence", "Sequence to call when timer expires"), 
			                             new GUIContent("<None>"), 
			                             fungusScript);
			
			serializedObject.ApplyModifiedProperties();
		}
	}
}
