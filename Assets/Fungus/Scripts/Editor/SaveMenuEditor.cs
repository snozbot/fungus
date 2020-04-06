// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEditor;

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
                    SaveManager.Delete(saveMenu.SaveDataKey);
                    FlowchartWindow.ShowNotification("Deleted Save Data");
                }
            }

            base.OnInspectorGUI();
        }
    }
}

#endif