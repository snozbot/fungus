using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// You can use this to be notified about various events in the localization process.
    /// </summary>
    public static class LocalizationSignals
    {
        /// <summary>
        /// Sent when a Localization component gets initialized.
        /// </summary>
        public static event LocalizationHandler Initialized = delegate { };
        public delegate void LocalizationHandler(Localization localization);
        public static void DoLocationInitialized(Localization localization)
        {
            Initialized(localization);
        }

        /// <summary>
        /// Sent when the active language is changed.
        /// </summary>
        public static event LangChangeHandler LanguageChanged;
        public delegate void LangChangeHandler(string prevLangCode, string newLangCode);
        public static void DoLangChanged(string prevLangCode, string newLangCode)
        {
            LanguageChanged(prevLangCode, newLangCode);
        }
    }
}