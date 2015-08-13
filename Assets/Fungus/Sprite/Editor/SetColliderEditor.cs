using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{
	[CustomEditor (typeof(SetCollider))]
	public class SetColliderEditor : CommandEditor 
	{
		protected SerializedProperty targetObjectsProp;
		protected SerializedProperty targetTagProp;
		protected SerializedProperty activeStateProp;

		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			targetObjectsProp = serializedObject.FindProperty("targetObjects");
			targetTagProp = serializedObject.FindProperty("targetTag");
			activeStateProp = serializedObject.FindProperty("activeState");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			ReorderableListGUI.Title(new GUIContent("Target Objects", "Objects containing collider components (2D or 3D)"));
			ReorderableListGUI.ListField(targetObjectsProp);

			EditorGUILayout.PropertyField(targetTagProp);
			EditorGUILayout.PropertyField(activeStateProp);

			serializedObject.ApplyModifiedProperties();
		}
	}

}
