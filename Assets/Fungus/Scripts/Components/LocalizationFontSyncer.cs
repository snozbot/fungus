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
        [SerializeField] protected TMP_Font defaultFont = null;

        [SerializeField] protected bool setFontOnStart = true;

        //[SerializeField] LangFontLink[] fontLinks = { };
        [SerializeField] protected LangFontLinkSet fontLinkSet = null;
        protected LangFontLink[] FontLinks { get { return fontLinkSet.Links; } }
       

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
            for (int i = 0; i < FontLinks.Length; i++)
            {
                LangFontLink currentLink = FontLinks[i];
                if (currentLink.langCode == langCode)
                    return currentLink.font;
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