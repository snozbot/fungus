// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.IO;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// A user profile represents a player that has settings and save files for our game. We may have
    /// many players of our game on the same install, this provides the basis of keeping different
    /// player settings and saves separate and allowing multiple users of the game to share the same
    /// operating system user data location.
    /// </summary>
    public class UserProfileManager
    {
        private const string LastUserDataFileName = "last_user.json";

        private const string ProfileDataFileName = "user_data.json";

        public string CurrentUserProfileName { get; private set; }

        public ISaveHandler CurrentUserProfileSaveHandler { get; set; } = DefaultUserProfileSaveHandler.CreateDefaultWithSerializers();

        /// <summary>
        /// Stores the user data profile after it is loaded and immediately before it is saved.
        /// </summary>
        public SaveData LastLoadedProfileData { get; protected set; }

        public void ChangeProfile(string saveProfileKey)
        {
            SaveProfileData();

            CurrentUserProfileName = saveProfileKey;

            var userFile = GetCurrentUserProfileFileName();
            Directory.CreateDirectory(Path.GetDirectoryName(userFile));
            if (File.Exists(userFile))
            {
                try
                {
                    var sdJSON = File.ReadAllText(userFile);
                    var saveData = CurrentUserProfileSaveHandler.DecodeFromJSON(sdJSON);
                    CurrentUserProfileSaveHandler.LoadSaveData(saveData);

                    LastLoadedProfileData = saveData;
                }
                catch (System.Exception)
                {
                }
            }

            if (LastLoadedProfileData == null)
            {
                LastLoadedProfileData = CurrentUserProfileSaveHandler.CreateSaveData(CurrentUserProfileName, string.Empty);
            }

            UserProfileManagerSignals.DoUserProfileChanged();
        }

        public string GetCurrentUserProfileDirectory()
        {
            return Path.GetFullPath(FungusConstants.StorageDirectory + CurrentUserProfileName + "/");
        }

        public string GetCurrentUserProfileFileName()
        {
            return Path.GetFullPath(GetCurrentUserProfileDirectory() + ProfileDataFileName);
        }

        public void Init()
        {
            //load last used profile
            try
            {
                var fileName = GetLastUserFile();
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

        public void ResetProfile()
        {
            LastLoadedProfileData = CurrentUserProfileSaveHandler.CreateSaveData(CurrentUserProfileName, string.Empty);
            ChangeProfile(CurrentUserProfileName);
        }

        public void SaveProfileData()
        {
            if (string.IsNullOrEmpty(CurrentUserProfileName))
                return;

            var userProfileSave = CurrentUserProfileSaveHandler.CreateSaveData(CurrentUserProfileName, string.Empty);

            LastLoadedProfileData = userProfileSave;

            UserProfileManagerSignals.DoUserProfileChangedPreSave();

            var fileName = GetLastUserFile();
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var last = new LastUserProfileUsedData() { lastUserProfileName = CurrentUserProfileName };
            File.WriteAllText(fileName, JsonUtility.ToJson(last));

            var sdJSON = CurrentUserProfileSaveHandler.EncodeToJSON(userProfileSave);

            File.WriteAllText(GetCurrentUserProfileFileName(), sdJSON);
        }

        private string GetLastUserFile()
        {
            return Path.GetFullPath(FungusConstants.StorageDirectory + LastUserDataFileName);
        }

        [System.Serializable]
        protected class LastUserProfileUsedData
        {
            public string lastUserProfileName;
        }
    }
}
