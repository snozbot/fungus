// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// An item of localizeable text.
    /// </summary>
    public interface ILocalizable
    {
        /// <summary>
        /// Gets the standard (non-localized) text.
        /// </summary>
        string GetStandardText();

        /// <summary>
        /// Sets the standard (non-localized) text.
        /// </summary>
        /// <param name="standardText">Standard text.</param>
        void SetStandardText(string standardText);

        /// <summary>
        /// Gets the description used to help localizers.
        /// </summary>
        /// <returns>The description.</returns>
        string GetDescription();

        /// <summary>
        /// Gets the unique string identifier.
        /// </summary>
        string GetStringId();
    }
}
