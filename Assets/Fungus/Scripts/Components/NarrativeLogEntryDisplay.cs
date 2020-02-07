// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Displays Narrative Log Entries in the UI.
    /// </summary>
    public class NarrativeLogEntryDisplay : MonoBehaviour
    {
        [SerializeField] protected Text nameTextField;
        [SerializeField] protected Text storyTextField;
        private NarrativeLogEntry toDisplay;

        /// <summary>
        /// The entry that this represents in the UI. The setter here alters this display's UI fields
        /// to fit the new value.
        /// </summary>
        public virtual NarrativeLogEntry ToDisplay
        {
            get { return toDisplay; }
            set
            {
                toDisplay = value;
                UpdateDisplays();
            }
        }

        /// <summary>
        /// Ensures that the UI components of this display fit the NarrativeLog Entry
        /// it has... if any.
        /// </summary>
        protected virtual void UpdateDisplays()
        {
            if (ToDisplay == null)
            {
                nameTextField.text = "";
                storyTextField.text = "";
            }
            else
            {
                nameTextField.text = toDisplay.name;
                storyTextField.text = toDisplay.text;
            }
        }
    }
}