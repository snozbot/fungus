/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

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
