// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(SaveMenu), true)]
    public class SaveMenuEditor : Editor 
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(new GUIContent("Delete Save Data", "Deletes the save data associated with the Save Data Key from PlayerPrefs")))
            {
                var saveMenu = target as SaveMenu;

                if (saveMenu != null)
                {
                    PlayerPrefs.DeleteKey(saveMenu.SaveDataKey);
                    FlowchartWindow.ShowNotification("Deleted Save Data");
                }
            }

            base.OnInspectorGUI();
        }
    }
}

#endif