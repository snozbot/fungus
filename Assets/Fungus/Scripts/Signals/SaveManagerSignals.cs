// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Save manager signalling system.
    /// You can use this to be notified about various events in the save game system.
    /// </summary>
    public static class SaveManagerSignals
    {
        #region Public members

        /// <summary>
        /// GameSaved signal. Sent just after the game is saved.
        /// </summary>
        public static event GameSavedHandler OnGameSaved;
        public delegate void GameSavedHandler(string saveDataKey);
        public static void DoGameSaved(string saveDataKey) { if (OnGameSaved != null) OnGameSaved(saveDataKey); }

        /// <summary>
        /// GameLoaded signal. Sent just after the game is loaded.
        /// </summary>
        public static event GameLoadedHandler OnGameLoaded;
        public delegate void GameLoadedHandler(string saveDataKey);
        public static void DoGameLoaded(string saveDataKey) { if (OnGameLoaded != null) OnGameLoaded(saveDataKey); }

        /// <summary>
        /// SavePointAdded signal. Sent when a new save point is added to the save history (typically via the Save Point command).
        /// </summary>
        public static event SavePointAddedHandler OnSavePointAdded;
        public delegate void SavePointAddedHandler(string savePointKey, string savePointDescription);
        public static void DoSavePointAdded(string savePointKey, string savePointDescription) { if (OnSavePointAdded != null) OnSavePointAdded(savePointKey, savePointDescription); }

        #endregion
    }
}
