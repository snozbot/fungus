using UnityEngine;
using UnityEngine.UI;


namespace CGTUnity.Fungus.NarrativeLogSystem
{
    /// <summary>
    /// Displays Narrative Log Entries in the UI.
    /// </summary>
    public class NarrativeLogEntryDisplay : MonoBehaviour
    {
        [SerializeField] protected Text nameTextField;
        [SerializeField] protected Text storyTextField;
        Entry toDisplay;

        /// <summary>
        /// The entry that this represents in the UI. The setter here alters this display's UI fields
        /// to fit the new value.
        /// </summary>
        public virtual Entry ToDisplay
        {
            get                                     { return toDisplay; }
            set 
            {
                toDisplay =                         value;
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
                nameTextField.text =                "";
                storyTextField.text =               "";
            }
            else
            {
                nameTextField.text =                toDisplay.NameText;
                storyTextField.text =               toDisplay.StoryText;
            }
        }

    }
}