// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// </summary>
    public static class UserProfileManagerSignals
    {
        /// <summary>
        /// </summary>
        public static event System.Action OnUserProfileChanged;

        public static void DoUserProfileChanged()
        {
            OnUserProfileChanged?.Invoke();
        }

        /// <summary>
        /// </summary>
        public static event System.Action OnUserProfileChangedPreSave;

        public static void DoUserProfileChangedPreSave()
        {
            OnUserProfileChangedPreSave?.Invoke();
        }
    }
}
