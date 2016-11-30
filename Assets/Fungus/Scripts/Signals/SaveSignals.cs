// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Save manager signalling system.
    /// You can use this to be notified about various events in the save game system.
    /// </summary>
    public static class SaveSignals
    {
        #region Public members

        /// <summary>
        /// GameSave signal. Sent when the game is saved.
        /// </summary>
        public static event GameSaveHandler OnGameSave;
        public delegate void GameSaveHandler(string saveKey, string saveDescription);
        public static void DoGameSave(string saveKey, string saveDescription) { if (OnGameSave != null) OnGameSave(saveKey, saveDescription); }

        #endregion
    }
}
