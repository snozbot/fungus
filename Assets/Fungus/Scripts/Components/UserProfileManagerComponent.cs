// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public class UserProfileManagerComponent : MonoBehaviour
    {
        public UserProfileManager UserProfileManager { get; private set; }

        public void Awake()
        {
            UserProfileManager = new UserProfileManager();
            UserProfileManager.Init();
        }

        public void ChangeProfile(string saveProfileKey)
        {
            UserProfileManager.ChangeProfile(saveProfileKey);
        }

        public string CurrentUserProfileName => UserProfileManager.CurrentUserProfileName;

        public string GetCurrentUserProfileDirectory() => UserProfileManager.GetCurrentUserProfileDirectory();

        private void OnDestroy()
        {
            SaveProfileData();
        }

        public void SaveProfileData()
        {
            UserProfileManager.SaveProfileData();
        }
    }
}
