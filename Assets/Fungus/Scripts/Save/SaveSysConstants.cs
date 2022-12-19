using UnityEngine;

namespace Fungus
{
    public static class SaveSysConstants
    {
        public static string SceneNameKey { get { return "SceneName"; } }
        public static string SaveDescKey { get { return "SaveDesc"; } }
        public static string SlotSavePrefix { get { return "Save"; } }
        public static string AutoSavePrefix { get { return "AutoSave"; } }

        public static int CurrentSaveGameDataVersion { get { return 1; } }
        public static int CurrentProfileDataVersion { get { return 1; } }
        public static string StorageDirectory { get { return Application.dataPath; } }
    }
}