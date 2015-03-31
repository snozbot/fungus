using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(Call))]
	public class CallEditor : CommandEditor 
	{
		protected SerializedProperty targetBlockProp;

		protected virtual void OnEnable()
		{
			targetBlockProp = serializedObject.FindProperty("targetBlock");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			Call t = target as Call;

			Flowchart flowchart = t.GetFlowchart();
			if (flowchart == null)
			{
				return;
			}

			BlockEditor.BlockField(targetBlockProp,
			                             new GUIContent("Target Block", "Block to call"), 
										 new GUIContent("<Continue>"), 
										 flowchart);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
