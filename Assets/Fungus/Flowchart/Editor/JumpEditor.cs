using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(Jump))]
	public class JumpEditor : CommandEditor 
	{
		protected SerializedProperty targetLabelProp;

		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			targetLabelProp = serializedObject.FindProperty("targetLabel");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			Jump t = target as Jump;

			LabelEditor.LabelField(targetLabelProp,
			                       new GUIContent("Target Label", "Label to jump to"), 
								   t.parentBlock);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
