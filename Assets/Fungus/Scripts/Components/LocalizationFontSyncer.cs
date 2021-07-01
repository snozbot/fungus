using UnityEngine;
using System.Collections.Generic;
#if UNITY_2018_1_OR_NEWER
using TMPro;
using TMPFont = TMPro.TMP_FontAsset;
#endif

namespace Fungus
{
    /// <summary>
    /// Automatically changes the font of the text field (UGUI, TMPRroUGUI or TMPro) component 
    /// automatically when the Localization's active language is set.
    /// </summary>
    public class LocalizationFontSyncer : MonoBehaviour
    {
        [Tooltip("The GameObject with the text field component. If null, this will be set to the current GameObject.")]
        [SerializeField] protected GameObject withTextField = null;

        protected TextAdapter textField = new TextAdapter();

        [Header("When a language is set that isn't in the font links.")]
        [Tooltip("For when using Unity UGUI text fields.")]
        [SerializeField] protected Font defaultFont = null;
#if UNITY_2018_1_OR_NEWER
        [Tooltip("For when using TextMeshPro or TextMeshPro (Text UI) text fields. If set, takes precendence over the UGUI default font.")]
        [SerializeField] protected TMPFont defaultTMPFont = null;
#endif

        [SerializeField] protected bool setFontOnStart = true;

        [SerializeField] protected LangFontLinkSet fontLinkSet = null;
        protected IList<LangFontLink> FontLinks { get { return fontLinkSet.Links; } }
       
        protected virtual void Awake()
        {
            EnsureTextFieldAvailable();
            textField.InitFromGameObject(withTextField);
            ListenForLocalizationSignals();
        }

        protected virtual void EnsureTextFieldAvailable()
        {
            if (withTextField == null)
                withTextField = this.gameObject;

            bool stillNothing = withTextField == null;
            if (stillNothing)
            {
                LetTheUserKnow();
                return;
            }
        }

        protected virtual void LetTheUserKnow()
        {
            string errorMessage = this.name + @" needs a reference to a GameObject with a Unity Text,
TextMeshPro, or TextMeshProUGUI component!";
            throw new System.MissingFieldException(errorMessage);
        }
                
        protected virtual void ListenForLocalizationSignals()
        {
            LocalizationSignals.LanguageChanged += OnLanguageChanged;
        }

        protected virtual void OnLanguageChanged(string prevLangCode, string newLangCode)
        {
            SetFontForLanguage(newLangCode);
        }

        public virtual void SetFontForLanguage(string lang)
        {
            FontAdapter font = GetFontForLanguage(lang);
            textField.Font = font;
        }

        FontAdapter GetFontForLanguage(string lang)
        {
            for (int i = 0; i < FontLinks.Count; i++)
            {
                LangFontLink currentLink = FontLinks[i];
                if (currentLink.LangCode == lang)
                {
                    // For some reason, even if we refresh all the links in Awake here,
                    // the Links' Font properties get set to null... I have no idea why. 
                    // So, we have to force a refresh here to make sure the Font property
                    // doesn't give us Le Zilch(tm).
                    currentLink.Refresh(); 
                    
                    return currentLink.Font;
                }
            }

#if UNITY_2018_1_OR_NEWER
            if (defaultTMPFont != null)
                return defaultTMPFont;
#endif

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