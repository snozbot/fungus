using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(Call))]
	public class CallEditor : CommandEditor 
	{
		protected SerializedProperty targetFlowchartProp;
		protected SerializedProperty targetBlockProp;
		protected SerializedProperty callModeProp;

		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			targetFlowchartProp = serializedObject.FindProperty("targetFlowchart");
			targetBlockProp = serializedObject.FindProperty("targetBlock");
			callModeProp = serializedObject.FindProperty("callMode");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			Call t = target as Call;

			Flowchart flowchart = null;
			if (targetFlowchartProp.objectReferenceValue == null)
			{
				flowchart = t.GetFlowchart();
			}
			else
			{
				flowchart = targetFlowchartProp.objectReferenceValue as Flowchart;
			}

			EditorGUILayout.PropertyField(targetFlowchartProp);

			if (flowchart != null)
			{
				BlockEditor.BlockField(targetBlockProp,
				                       new GUIContent("Target Block", "Block to call"), 
									   new GUIContent("<None>"), 
									   flowchart);
			}

			EditorGUILayout.PropertyField(callModeProp);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
