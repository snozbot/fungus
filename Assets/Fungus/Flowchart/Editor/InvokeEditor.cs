using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	[CustomEditor (typeof(Invoke))]
	public class InvokeEditor : CommandEditor 
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

			switch ((Invoke.InvokeType)invokeTypeProp.enumValueIndex)
			{
			case Invoke.InvokeType.Static:
				EditorGUILayout.PropertyField(staticEventProp);
				break;
			case Invoke.InvokeType.DynamicBoolean:
				EditorGUILayout.PropertyField(booleanEventProp);
				EditorGUILayout.PropertyField(booleanParameterProp);
				break;
			case Invoke.InvokeType.DynamicInteger:
				EditorGUILayout.PropertyField(integerEventProp);
				EditorGUILayout.PropertyField(integerParameterProp);
				break;
			case Invoke.InvokeType.DynamicFloat:
				EditorGUILayout.PropertyField(floatEventProp);
				EditorGUILayout.PropertyField(floatParameterProp);
				break;
			case Invoke.InvokeType.DynamicString:
				EditorGUILayout.PropertyField(stringEventProp);
				EditorGUILayout.PropertyField(stringParameterProp);
				break;
			}

			serializedObject.ApplyModifiedProperties();
		}
	}
}
