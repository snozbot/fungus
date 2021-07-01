using UnityEngine;
using UnityEngine.UI;
#if UNITY_2018_1_OR_NEWER
using TMPro;
using TMPFont = TMPro.TMP_FontAsset;
#endif

namespace Fungus
{
    /// <summary>
    /// Adapter for Unity's built-in Font classes.
    /// </summary>
    public class FontAdapter
    {
        protected Font forUGUI = null;
#if UNITY_2018_1_OR_NEWER
        protected TMPFont forTMP = null;
#endif

        public virtual void SetFrom(Text textField)
        {
            EnsureValidityOf(textField);
            SetFrom(textField.font);
        }

        protected virtual void EnsureValidityOf(object textField)
        {
            if (textField == null)
                throw new System.ArgumentNullException("Can't set a FontAdapter from a null text field!");
        }

        public virtual void SetFrom(Font font)
        {
            forUGUI = font;
            forTMP = null;
        }

#if UNITY_2018_1_OR_NEWER
        public virtual void SetFrom(TMP_Text textField)
        {
            EnsureValidityOf(textField);
            SetFrom(textField.font);
        }

        public virtual void SetFrom(TMPFont tmpFont)
        {
            forTMP = tmpFont;
            forUGUI = tmpFont.sourceFontFile;
        }

#endif

        public virtual void SetFrom(FontAdapter otherAdapter)
        {
            this.forUGUI = otherAdapter.forUGUI;
            this.forTMP = otherAdapter.forTMP;
        }

        // We want this to be able to be treated as a Font or TMPFont, at least as far as
        // value-assignment and creation goes.
        public static implicit operator Font(FontAdapter fontAdapter)
        {
            return fontAdapter.forUGUI;
        }

        // This is so we can make a FontAdapter by assigning a UGUI Font to a variable of the 
        // former, so we don't have to first create a FontAdapter, and then pass the Font to 
        // its SetFrom method. Same logic for the TMPFont equivalent of this.
        public static implicit operator FontAdapter(Font font)
        {
            return new FontAdapter()
            {
                forUGUI = font
            };
        }

#if UNITY_2018_1_OR_NEWER
        public static implicit operator TMPFont(FontAdapter fontAdapter)
        {
            return fontAdapter.forTMP;
        }

        public static implicit operator FontAdapter(TMPFont tmpFont)
        {
            return new FontAdapter()
            {
                forTMP = tmpFont,
                forUGUI = tmpFont.sourceFontFile
            };
        }
#endif

    }
}