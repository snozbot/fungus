// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// One of the possible ways to route text from a writer to a desired location.
    /// </summary>
    public interface IWriterTextDestination
    {
        string Text { get; set; }

        void ForceRichText();
        void SetTextColor(Color textColor);
        void SetTextAlpha(float textAlpha);
        bool HasTextObject();
        bool SupportsRichText();
    }
}