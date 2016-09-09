using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Presents multiple choice buttons to the players.
    /// </summary>
    public interface IMenuDialog
    {
        /// <summary>
        /// Sets the active state of the Menu Dialog gameobject.
        /// </summary>
        void SetActive(bool state);

        /// <summary>
        /// Adds the option to the list of displayed options.
        /// Will cause the Menu dialog to become visible if it is not already visible.
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        /// <param name="text">The option text to display on the button.</param>
        /// <param name="interactable">If false, the option is displayed but is not selectable.</param>
        /// <param name="targetBlock">Block to execute when the option is selected.</param>
        bool AddOption(string text, bool interactable, Block targetBlock);

        /// <summary>
        /// Show a timer during which the player can select an option.
        /// </summary>
        /// <param name="duration">The duration during which the player can select an option.</param>
        /// <param name="targetBlock">Block to execute if the player does not select an option in time.</param>
        void ShowTimer(float duration, Block targetBlock);

        /// <summary>
        /// Returns true if the Menu Dialog is currently displayed.
        /// </summary>
        bool IsActive();

        /// <summary>
        /// Returns the number of currently displayed options.
        /// </summary>
        int DisplayedOptionsCount { get; }
    }
}