// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

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

        protected override void DrawProperties()
        {
            ReorderableListGUI.Title("Save Point Keys");
            ReorderableListGUI.ListField(savePointKeysProp);
        }
    }
}

#endif