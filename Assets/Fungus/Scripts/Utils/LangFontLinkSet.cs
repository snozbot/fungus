using UnityEngine;
using System.Collections.Generic;
using TMP_Font = TMPro.TMP_FontAsset;

namespace Fungus
{
    [CreateAssetMenu(fileName = "NewLangFontLinkSet", menuName = "Fungus/Localization/LangFontLinkSet")]
    public class LangFontLinkSet : ScriptableObject
    {
        [SerializeField]
        LangFontLink[] links = { };

        public IList<LangFontLink> Links { get { return links; } }

    }

    /// <summary>
    /// Encapsulates a link between language codes and font assets to use for
    /// them, since Unity doesn't quite support Dictionaries very well.
    /// </summary>
    [System.Serializable]
    public struct LangFontLink
    {
        public LangFontLink(string langCode = "")
        {
            this.langCode = langCode;
            uguiFont = null;
#if UNITY_2018_1_OR_NEWER
            tmpFont = null;
#endif
            this.font = new FontAdapter();
        }

        [SerializeField] string langCode;
        [SerializeField] Font uguiFont;

#if UNITY_2018_1_OR_NEWER
        [Tooltip("If this is set, this will take precendence even for Unity UGUI Text components.")]
        [SerializeField] TMP_Font tmpFont;
#endif
        private FontAdapter font;

        public LangFontLink(string langCode, Font font) : this(langCode)
        {
            uguiFont = font;
            this.font.SetFrom(font);
        }

#if UNITY_2018_1_OR_NEWER
        public LangFontLink(string langCode, TMP_Font font) : this(langCode)
        {
            tmpFont = font;
            uguiFont = font.sourceFontFile;
            this.font.SetFrom(font);
        }
#endif

        public string LangCode
        {
            get { return langCode; }
            set { langCode = value; }
        }

        public Font UGUIFont
        {
            get { return uguiFont; }
            set { uguiFont = value; }
        }

        public TMP_Font TMPFont
        {
            get { return tmpFont; }
            set { tmpFont = value; }
        }

        public FontAdapter Font
        {
            get { return font; }
        }

        /// <summary>
        /// Necessary when dealing with LangFontLinks set up in the Inspector, since the user may have
        /// different font families set in them. Best call this before using those instances.
        /// </summary>
        public void Refresh()
        {
            // Despite the base constructor here, the adapter may not have been instantiated by Unity
            font = new FontAdapter();

#if UNITY_2018_1_OR_NEWER
            if (tmpFont != null)
            {
                font.SetFrom(tmpFont);
                return;
            }
#endif

            if (uguiFont != null)
                font.SetFrom(uguiFont);
        }



    }

}