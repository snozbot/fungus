// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif
using UnityEngine.Serialization;

namespace Fungus
{
 
#if UNITY_LOCALIZATION
    /// <summary>
    /// Set the active language.
    /// </summary>
    [CommandInfo("Narrative", 
        "Set Language", 
        "Set the active language.")]
#else
    /// <summary>
    /// Set the active language for the scene. A Localization object with a localization file must be present in the scene.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Set Language", 
                 "Set the active language for the scene. A Localization object with a localization file must be present in the scene.")]
#endif
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SetLanguage : Command
    {
        [Tooltip("Code of the language to set. e.g. ES, DE, JA")]
        [SerializeField] protected StringData _languageCode = new StringData(); 

        #region Public members

        public static string mostRecentLanguage = "";

        public override void OnEnter()
        {
#if UNITY_LOCALIZATION
            var locale = LocalizationSettings.AvailableLocales.GetLocale((string)_languageCode);
            LocalizationSettings.SelectedLocale = locale; 
#else
            Localization localization = GameObject.FindObjectOfType<Localization>();
            if (localization != null)
            {
                localization.SetActiveLanguage(_languageCode.Value, true);

                // Cache the most recently set language code so we can continue to 
                // use the same language in subsequent scenes.
                mostRecentLanguage = _languageCode.Value;
            }
#endif
            
            Continue();
        }

        public override string GetSummary()
        {
            return _languageCode.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _languageCode.stringRef == variable || base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("languageCode")] public string languageCodeOLD = "";

        protected virtual void OnEnable()
        {
            if (languageCodeOLD != "")
            {
                _languageCode.Value = languageCodeOLD;
                languageCodeOLD = "";
            }
        }

        #endregion
    }
}