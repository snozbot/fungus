using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SavePointLoaded), true)]
    public class SavePointLoadedEditor : EventHandlerEditor 
    {
        protected SerializedProperty savePointKeysProp;

        protected virtual void OnEnable()
        {
            savePointKeysProp = serializedObject.FindProperty("savePointKeys");
        }

        public override void DrawInspectorGUI()
        {
            serializedObject.Update();

            ReorderableListGUI.Title("Save Point Keys");
            ReorderableListGUI.ListField(savePointKeysProp);

            DrawHelpBox();

            serializedObject.ApplyModifiedProperties();
        }
    }
}