// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SaveData), true)]
    public class SaveDataEditor : Editor 
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

#endif