// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Save manager signaling system.
    /// You can use this to be notified about various events in the save game system.
    /// </summary>
    public static class SaveManagerSignals
    {
        #region Public members

        /// <summary>
        /// SavePointLoaded signal. Sent just after a SavePoint is loaded.
        /// </summary>
        public static event SavePointLoadedHandler OnSavePointLoaded;
        public delegate void SavePointLoadedHandler(string savePointKey);
        public static void DoSavePointLoaded(string savePointKey) { if (OnSavePointLoaded != null) OnSavePointLoaded(savePointKey); }

        /// <summary>
        /// SavePointAdded signal. Sent when a new save point is added to the save history (typically via the Save Point command).
        /// </summary>
        public static event SavePointAddedHandler OnSavePointAdded;
        public delegate void SavePointAddedHandler(string savePointKey, string savePointDescription);
        public static void DoSavePointAdded(string savePointKey, string savePointDescription) { if (OnSavePointAdded != null) OnSavePointAdded(savePointKey, savePointDescription); }

        /// <summary>
        /// SaveReset signal. Sent when the save history is reset.
        /// </summary>
        public static event SaveResetHandler OnSaveReset;
        public delegate void SaveResetHandler();
        public static void DoSaveReset() { if (OnSaveReset != null) OnSaveReset(); }

        #endregion
    }
}
