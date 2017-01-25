// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(InvokeEvent))]
    public class InvokeEventEditor : CommandEditor 
    {
        protected SerializedProperty descriptionProp;
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

            descriptionProp = serializedObject.FindProperty("description");
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

            EditorGUILayout.PropertyField(descriptionProp);
            EditorGUILayout.PropertyField(delayProp);
            EditorGUILayout.PropertyField(invokeTypeProp);

            switch ((InvokeType)invokeTypeProp.enumValueIndex)
            {
            case InvokeType.Static:
                EditorGUILayout.PropertyField(staticEventProp);
                break;
            case InvokeType.DynamicBoolean:
                EditorGUILayout.PropertyField(booleanEventProp);
                EditorGUILayout.PropertyField(booleanParameterProp);
                break;
            case InvokeType.DynamicInteger:
                EditorGUILayout.PropertyField(integerEventProp);
                EditorGUILayout.PropertyField(integerParameterProp);
                break;
            case InvokeType.DynamicFloat:
                EditorGUILayout.PropertyField(floatEventProp);
                EditorGUILayout.PropertyField(floatParameterProp);
                break;
            case InvokeType.DynamicString:
                EditorGUILayout.PropertyField(stringEventProp);
                EditorGUILayout.PropertyField(stringParameterProp);
                break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
