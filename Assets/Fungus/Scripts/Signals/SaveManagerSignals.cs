// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

//TODO do we need a save meta changed signal

namespace Fungus
{
    /// <summary>
    /// Save manager signaling system.
    /// You can use this to be notified about various events in the save game system.
    /// </summary>
    public static class SaveManagerSignals
    {
        /// <summary>
        /// ProgressMarker reached.
        /// </summary>
        public static event ProgressMarkerChangedHandler OnProgressMarkerReached;

        public delegate void ProgressMarkerChangedHandler(ProgressMarker was, ProgressMarker now);

        public static void DoProgressMarkerReached(ProgressMarker was, ProgressMarker now)
        {
            if (OnProgressMarkerReached != null) OnProgressMarkerReached(was, now);
        }

        /// <summary>
        /// OnSavePrepare signal. Sent when a new save is requested, allowing others to make preparations before being
        /// serialised.
        /// </summary>
        public static event SaveNameDescriptionHandler OnSavePrepare;

        public delegate void SaveNameDescriptionHandler(string saveName, string saveDescription);

        public static void DoSavePrepare(string savePointKey, string savePointDescription)
        {
            if (OnSavePrepare != null) OnSavePrepare(savePointKey, savePointDescription);
        }

        /// <summary>
        /// SavePointAdded signal. Sent when a new save point is added to the save history (typically via the Save Point command).
        /// </summary>
        public static event SaveNameDescriptionHandler OnSaveSaved;

        public static void DoSaveSaved(string savePointKey, string savePointDescription)
        {
            if (OnSaveSaved != null) OnSaveSaved(savePointKey, savePointDescription);
        }

        /// <summary>
        /// SavePointLoaded signal. Sent just before a SavePoint is loaded, before the scene is switched.
        /// </summary>
        public static event SaveNameHandler OnSavePreLoad;

        public delegate void SaveNameHandler(string savePointKey);

        public static void DoSavePreLoad(string savePointKey)
        {
            if (OnSavePreLoad != null) OnSavePreLoad(savePointKey);
        }

        /// <summary>
        /// SavePointLoaded signal. Sent just after a SavePoint is loaded.
        /// </summary>
        public static event SaveNameHandler OnSaveLoaded;

        public static void DoSaveLoaded(string savePointKey)
        {
            if (OnSaveLoaded != null) OnSaveLoaded(savePointKey);
        }

        /// <summary>
        /// SavePointLoaded signal. Sent just after a SavePoint is loaded.
        /// </summary>
        public static event SaveNameHandler OnSaveDeleted;

        public static void DoSaveDeleted(string savePointKey)
        {
            if (OnSaveDeleted != null) OnSaveDeleted(savePointKey);
        }

        /// <summary>
        /// SaveReset signal. Sent when the saves for the current profile are removed or progress reset or profile changed.
        /// </summary>
        /// </summary>
        public static event SaveResetHandler OnSaveReset;

        public delegate void SaveResetHandler();

        public static void DoSaveReset()
        {
            if (OnSaveReset != null) OnSaveReset();
        }

        /// <summary>
        /// Saving or Loading allowed changed signal.
        /// </summary>
        public static event SavingLoadingAllowedChangeHandler OnSavingLoadingAllowedChanged;

        public delegate void SavingLoadingAllowedChangeHandler();

        public static void DoSavingLoadingAllowedChanged()
        {
            if (OnSavingLoadingAllowedChanged != null) OnSavingLoadingAllowedChanged();
        }
    }
}