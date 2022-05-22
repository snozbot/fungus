// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(OrthographicCameraUtility))]
    public class OrthographicCameraUtilityEditor : CommandEditor 
    {
        protected SerializedProperty actionProp;
        protected SerializedProperty stateProp;
        protected SerializedProperty rotateProp;
        protected SerializedProperty rotateValProp;
        protected SerializedProperty rotateValVec3Prop;
        protected SerializedProperty rotateDurProp;
        protected SerializedProperty minProp;
        protected SerializedProperty maxProp;
        protected SerializedProperty speedProp;
        protected SerializedProperty velocityProp;
        protected SerializedProperty smoothProp;
        protected SerializedProperty smoothDampProp;
        protected SerializedProperty velocityVec3Prop;
        protected SerializedProperty camProp;
        protected SerializedProperty rightMouseResetProp;
        protected SerializedProperty targetObjProp;

        public override void OnEnable()
        {
            base.OnEnable();

            actionProp = serializedObject.FindProperty("action");
            stateProp = serializedObject.FindProperty("activeState");
            rotateProp = serializedObject.FindProperty("rotate");
            rotateValProp = serializedObject.FindProperty("rotateValue");
            rotateValVec3Prop = serializedObject.FindProperty("rotateVector3");
            rotateDurProp = serializedObject.FindProperty("duration");
            minProp = serializedObject.FindProperty("minValue");
            maxProp = serializedObject.FindProperty("maxValue");
            speedProp = serializedObject.FindProperty("speed");
            velocityProp = serializedObject.FindProperty("velocity");
            smoothProp = serializedObject.FindProperty("smoothness");
            smoothDampProp = serializedObject.FindProperty("smoothDamp");
            velocityVec3Prop = serializedObject.FindProperty("velocityVec3");
            camProp = serializedObject.FindProperty("targetCamera");
            rightMouseResetProp = serializedObject.FindProperty("rightMouseReset");
            targetObjProp = serializedObject.FindProperty("targetObject");
        }

        public override void DrawCommandGUI()
        {
            serializedObject.Update();

            OrthographicCameraUtility t = target as OrthographicCameraUtility;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(actionProp);
            if(t.action != CameraUtilSelect.Rotate)
            {
                EditorGUILayout.PropertyField(stateProp);
            }
            if(t.action == CameraUtilSelect.Rotate)
            {
                EditorGUILayout.PropertyField(rotateProp);
                EditorGUILayout.PropertyField(rotateDurProp);
            }
            if(t.rotate == CameraUtilRotate.RotateX || t.rotate == CameraUtilRotate.RotateY || t.rotate == CameraUtilRotate.RotateZ)
            {
                if(t.action == CameraUtilSelect.Rotate)
                EditorGUILayout.PropertyField(rotateValProp);
            }
            if(t.rotate == CameraUtilRotate.Rotate)
            {
                EditorGUILayout.PropertyField(rotateValVec3Prop);
            }
            if(t.action == CameraUtilSelect.ScrollPinchToZoom)
            {
                EditorGUILayout.PropertyField(minProp);
                EditorGUILayout.PropertyField(maxProp);                
                EditorGUILayout.PropertyField(velocityProp);
                EditorGUILayout.PropertyField(speedProp);
            }

            if(t.action == CameraUtilSelect.CameraFollow)
            {
                EditorGUILayout.PropertyField(targetObjProp);
            }

            if(t.action == CameraUtilSelect.DragCamera)
            {
                EditorGUILayout.PropertyField(smoothDampProp);
                EditorGUILayout.PropertyField(velocityVec3Prop);
            }
            if(t.action != CameraUtilSelect.Rotate && t.action != CameraUtilSelect.None)
            {
                EditorGUILayout.PropertyField(smoothProp);
            }
            EditorGUILayout.PropertyField(camProp);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
