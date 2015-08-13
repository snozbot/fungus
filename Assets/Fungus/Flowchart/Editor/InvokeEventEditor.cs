using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(InvokeEvent))]
	public class InvokeEventEditor : CommandEditor 
	{
		protected SerializedProperty delayProp;
		protected SerializedProperty invokeTypeProp;
		protected SerializedProperty staticEventProp;
		protected SerializedProperty booleanParameterProp;
		protected SerializedProperty booleanEventProp;
		protected SerializedProperty integerParameterProp;
		protected SerializedProperty integerEventProp;
		protected SerializedProperty floatParameterProp;
		protected SerializedProperty floatEventProp;
		protected SerializedProperty stringParameterProp;
		protected SerializedProperty stringEventProp;

		protected virtual void OnEnable()
		{
			if (NullTargetCheck()) // Check for an orphaned editor instance
				return;

			delayProp = serializedObject.FindProperty("delay");
			invokeTypeProp = serializedObject.FindProperty("invokeType");
			staticEventProp = serializedObject.FindProperty("staticEvent");
			booleanParameterProp = serializedObject.FindProperty("booleanParameter");
			booleanEventProp = serializedObject.FindProperty("booleanEvent");
			integerParameterProp = serializedObject.FindProperty("integerParameter");
			integerEventProp = serializedObject.FindProperty("integerEvent");
			floatParameterProp = serializedObject.FindProperty("floatParameter");
			floatEventProp = serializedObject.FindProperty("floatEvent");
			stringParameterProp = serializedObject.FindProperty("stringParameter");
			stringEventProp = serializedObject.FindProperty("stringEvent");
		}

		public override void DrawCommandGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(delayProp);
			EditorGUILayout.PropertyField(invokeTypeProp);

			switch ((InvokeEvent.InvokeType)invokeTypeProp.enumValueIndex)
			{
			case InvokeEvent.InvokeType.Static:
				EditorGUILayout.PropertyField(staticEventProp);
				break;
			case InvokeEvent.InvokeType.DynamicBoolean:
				EditorGUILayout.PropertyField(booleanEventProp);
				EditorGUILayout.PropertyField(booleanParameterProp);
				break;
			case InvokeEvent.InvokeType.DynamicInteger:
				EditorGUILayout.PropertyField(integerEventProp);
				EditorGUILayout.PropertyField(integerParameterProp);
				break;
			case InvokeEvent.InvokeType.DynamicFloat:
				EditorGUILayout.PropertyField(floatEventProp);
				EditorGUILayout.PropertyField(floatParameterProp);
				break;
			case InvokeEvent.InvokeType.DynamicString:
				EditorGUILayout.PropertyField(stringEventProp);
				EditorGUILayout.PropertyField(stringParameterProp);
				break;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
