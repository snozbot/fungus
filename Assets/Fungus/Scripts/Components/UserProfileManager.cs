// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    [AddComponentMenu("")]
    public class UserProfileManager : MonoBehaviour
    {
        [System.Serializable]
        protected class LastUserProfileUsedData
        {
            public string lastUserProfileName;
        }

        public static string StorageDirectory { get { return Application.persistentDataPath + "/Fungus/"; } }

        private const string LastUserDataFileName = "last_user.json";
        private const string ProfileDataFileName = "user_data.json";

        public string CurrentUserProfileName { get; private set; }

        public SaveData CurrentProfileData { get; protected set; }

        private string GetSaveManagerDataFile()
        {
            return Path.GetFullPath(StorageDirectory + LastUserDataFileName);
        }

        public string GetCurrentUserProfileDirectory()
        {
            return Path.GetFullPath(StorageDirectory + CurrentUserProfileName + "/");
        }

        public string GetCurrentUserProfileFileName()
        {
            return Path.GetFullPath(GetCurrentUserProfileDirectory() + ProfileDataFileName);
        }

        public void Awake()
        {
            //load last used profile
            try
            {
#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
                var fileName = GetSaveManagerDataFile();
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                var datString = File.ReadAllText(fileName);
                var dat = JsonUtility.FromJson<LastUserProfileUsedData>(datString);
                ChangeProfile(dat.lastUserProfileName);
            }
            catch (System.Exception)
            {
                //if that fails for whatever reason use default profile
                ChangeProfile(FungusConstants.DefaultSaveProfileKey);
            }
        }

        public void ChangeProfile(string saveProfileKey)
        {
            if (saveProfileKey != CurrentUserProfileName)
            {
                SaveProfileData();

                CurrentUserProfileName = saveProfileKey;
                CurrentProfileData = null;

                var userFile = GetCurrentUserProfileFileName();
                Directory.CreateDirectory(Path.GetDirectoryName(userFile));
                if (File.Exists(userFile))
                {
                    try
                    {
                        CurrentProfileData = JsonUtility.FromJson<SaveData>(File.ReadAllText(userFile));
                    }
                    catch (System.Exception)
                    {
                    }
                }

                if (CurrentProfileData == null)
                {
                    CurrentProfileData = new SaveData(CurrentUserProfileName) { version = FungusConstants.CurrentProfileDataVersion };
                }

#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
                UserProfileManagerSignals.DoUserProfileChanged();
            }
        }

        //delete profile


        private void OnDestroy()
        {
            SaveProfileData();
        }

        public void SaveProfileData()
        {
            if (CurrentProfileData == null || string.IsNullOrEmpty(CurrentUserProfileName))
                return;


            UserProfileManagerSignals.DoUserProfileChangedPreSave();

            var fileName = GetSaveManagerDataFile();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var last = new LastUserProfileUsedData() { lastUserProfileName = CurrentUserProfileName };
            File.WriteAllText(fileName, JsonUtility.ToJson(last));


            File.WriteAllText(GetCurrentUserProfileFileName(), JsonUtility.ToJson(CurrentProfileData));

#if UNITY_WEBGL
                Application.ExternalEval("_JS_FileSystem_Sync();");
#endif
        }
    }
}