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