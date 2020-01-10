// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;

namespace Fungus.EditorUtils
{
    public class SaveMenuItems 
    {
        [MenuItem("Tools/Fungus/Create/Save Menu", false, 1100)]
        static void CreateSaveMenu()
        {
            FlowchartMenuItems.SpawnPrefab("SaveMenu");
        }

        [MenuItem("Tools/Fungus/Create/Save Data", false, 1101)]
        static void CreateSaveData()
        {
            FlowchartMenuItems.SpawnPrefab("SaveData");
        }
    }
}