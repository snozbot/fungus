using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/*
	[CustomEditor (typeof(SendEvent))]
	public class SendEventEditor : CommandEditor 
	{
		protected SerializedProperty targetEventProp;
		protected SerializedProperty stopCurrentScriptProp;

		protected virtual void OnEnable()
		{
			targetEventProp = serializedObject.FindProperty("targetEvent");
			stopCurrentScriptProp = serializedObject.FindProperty("stopCurrentScript");
		}

		public override void DrawCommandGUI()
		{
			// For some reason the serializedObject has already been disposed by the time this method is called
			// The workaround is to acquire a new serializedObject and find the targetEvent property every time.
			// This could be a bug in the Unity 4.6 beta so try to aquire the property in OnEnable() again some time.

			serializedObject.Update();

			serializedObject.Update();

			EditorGUILayout.PropertyField(targetEventProp);
			EditorGUILayout.PropertyField(stopCurrentScriptProp);

			serializedObject.ApplyModifiedProperties();
		}
	}
	*/
}
