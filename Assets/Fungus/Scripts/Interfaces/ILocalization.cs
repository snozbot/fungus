// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Multi-language localization support.
    /// </summary>
    public interface ILocalization
    {
        /// <summary>
        /// Language to use at startup, usually defined by a two letter language code (e.g DE = German).
        /// </summary>
        string ActiveLanguage { get; }

        /// <summary>
        /// CSV file containing localization data which can be easily edited in a spreadsheet tool.
        /// </summary>
        TextAsset LocalizationFile { get; }

        /// <summary>
        /// Stores any notification message from export / import methods.
        /// </summary>
        string NotificationText { get; set; }

        /// <summary>
        /// Convert all text items and localized strings to an easy to edit CSV format.
        /// </summary>
        string GetCSVData();

        /// <summary>
        /// Scan a localization CSV file and copies the strings for the specified language code
        /// into the text properties of the appropriate scene objects.
        /// </summary>
        void SetActiveLanguage(string languageCode, bool forceUpdateSceneText = false);

        /// <summary>
        /// Populates the text property of a single scene object with a new text value.
        /// </summary>
        bool PopulateTextProperty(string stringId, string newText);

        /// <summary>
        /// Returns all standard text for localizeable text in the scene using an
        /// easy to edit custom text format.
        /// </summary>
        string GetStandardText();

        /// <summary>
        /// Sets standard text on scene objects by parsing a text data file.
        /// </summary>
        void SetStandardText(string textData);
    }

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
