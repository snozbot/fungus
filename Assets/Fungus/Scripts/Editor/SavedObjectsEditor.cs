using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SavedObjects), true)]
    public class SavedObjectsEditor : Editor 
    {
        protected SerializedProperty flowchartsProp;

        protected virtual void OnEnable()
        {
            flowchartsProp = serializedObject.FindProperty("flowcharts");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            ReorderableListGUI.Title("Flowcharts");
            ReorderableListGUI.ListField(flowchartsProp);

            serializedObject.ApplyModifiedProperties();
        }
    }
}