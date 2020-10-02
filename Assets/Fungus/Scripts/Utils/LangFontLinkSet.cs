using UnityEngine;
using TMP_Font = TMPro.TMP_FontAsset;

namespace Fungus
{

    [CreateAssetMenu(fileName = "NewLangFontLinkSet", menuName = "Fungus/Localization/LangFontLinkSet")]
    public class LangFontLinkSet : ScriptableObject
    {
        [SerializeField]
        LangFontLink[] links = { };

        public LangFontLink[] Links { get { return links; } }
    }

    /// <summary>
    /// Encapsulates a link between language codes and font assets to use for
    /// them, since Unity doesn't quite support Dictionaries very well.
    /// </summary>
    [System.Serializable]
    public struct LangFontLink
    {
        public string langCode;
        public TMP_Font font;

        public LangFontLink(string langCode, TMP_Font font)
        {
            this.langCode = langCode;
            this.font = font;
        }

    }

    
}