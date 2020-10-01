using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMP_Font = TMPro.TMP_FontAsset;

namespace Fungus
{
    /// <summary>
    /// Automatically changes the font of the TextMeshProUGUI component automatically when the
    /// Localization's active language is set.
    /// </summary>
    public class LocalizationFontSyncer : MonoBehaviour
    {
        [Tooltip("If null when the scene starts, this will try to get the component from the GameObject it's attached to.")]
        [SerializeField] protected TextMeshProUGUI textField = null;

        [Tooltip("When a language is set that isn't in the font links, this is the font switched to.")]
        [SerializeField] TMP_Font defaultFont = null;

        [SerializeField] bool setFontOnStart = true;

        [SerializeField] LangFontLink[] fontLinks = { };
       
        /// <summary>
        /// Encapsulates a link between language codes and font assets to use for
        /// them, since Unity doesn't quite support Dictionaries very well.
        /// </summary>
        [System.Serializable]
        public class LangFontLink
        {
            [SerializeField] protected string langCode = null;
            [SerializeField] protected TMP_Font font = null;

            public string LangCode { get { return langCode; } }
            public TMP_Font Font { get { return font; } }
        }

        protected virtual void Awake()
        {
            EnsureTextFieldAvailable();
            ListenForLocalizationSignals();
        }

        protected virtual void EnsureTextFieldAvailable()
        {
            if (textField == null)
                textField = GetComponent<TextMeshProUGUI>();

            bool stillNoTextField = textField == null;
            if (stillNoTextField)
                throw new System.MissingFieldException(this.name + " needs a TextMeshProUGUI component reference!");
        }

        protected virtual void ListenForLocalizationSignals()
        {
            LocalizationSignals.LanguageChanged += OnLanguageChanged;
        }

        protected virtual void OnLanguageChanged(string prevLangCode, string newLangCode)
        {
            SetFontForLanguage(newLangCode);
        }

        public virtual void SetFontForLanguage(string langCode)
        {
            TMP_Font font = GetFontForLanguage(langCode);
            textField.font = font;
        }

        TMP_Font GetFontForLanguage(string langCode)
        {
            for (int i = 0; i < fontLinks.Length; i++)
            {
                LangFontLink currentLink = fontLinks[i];
                if (currentLink.LangCode == langCode)
                    return currentLink.Font;
            }

            return defaultFont;
        }

        protected virtual void Start()
        {
            if (setFontOnStart)
                SetFontForLanguage(SetLanguage.mostRecentLanguage);
        }

        protected virtual void OnDestroy()
        {
            UnlistenForLocalizationSignals();
        }

        protected virtual void UnlistenForLocalizationSignals()
        {
            LocalizationSignals.LanguageChanged -= OnLanguageChanged;
        }

    }
}