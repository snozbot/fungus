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
        public static event SaveLoadedHandler OnSaveLoaded;
        public delegate void SaveLoadedHandler(string savePointKey);
        public static void DoSaveLoaded(string savePointKey) { if (OnSaveLoaded != null) OnSaveLoaded(savePointKey); }

        /// <summary>
        /// SavePointLoaded signal. Sent just after a SavePoint is loaded.
        /// </summary>
        public static event SaveDeletedHandler OnSaveDeleted;
        public delegate void SaveDeletedHandler(string savePointKey);
        public static void DoSaveDeleted(string savePointKey) { if (OnSaveDeleted != null) OnSaveDeleted(savePointKey); }

        /// <summary>
        /// SavePointAdded signal. Sent when a new save point is added to the save history (typically via the Save Point command).
        /// </summary>
        public static event SaveSavedHandler OnSaveSaved;
        public delegate void SaveSavedHandler(string savePointKey, string savePointDescription);
        public static void DoSaveSaved(string savePointKey, string savePointDescription) { if (OnSaveSaved != null) OnSaveSaved(savePointKey, savePointDescription); }

        /// <summary>
        /// SaveReset signal. Sent when the save history is reset.
        /// </summary>
        public static event SaveResetHandler OnSaveReset;
        public delegate void SaveResetHandler();
        public static void DoSaveReset() { if (OnSaveReset != null) OnSaveReset(); }


        /// <summary>
        /// ProgressMarker reached.
        /// </summary>
        public static event ProgressMarkerChangedHandler OnProgressMarkerReached;
        public delegate void ProgressMarkerChangedHandler(ProgressMarker was, ProgressMarker now);
        public static void DoProgressMarkerReached(ProgressMarker was, ProgressMarker now) { if (OnProgressMarkerReached != null) OnProgressMarkerReached(was, now); }

        #endregion
    }
}
