// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Presents multiple choice buttons to the players.
    /// </summary>
    public interface IMenuDialog
    {
        /// <summary>
        /// A cached list of button objects in the menu dialog.
        /// </summary>
        /// <value>The cached buttons.</value>
        Button[] CachedButtons { get; }

        /// <summary>
        /// A cached slider object used for the timer in the menu dialog.
        /// </summary>
        /// <value>The cached slider.</value>
        Slider CachedSlider { get; }

        /// <summary>
        /// Sets the active state of the Menu Dialog gameobject.
        /// </summary>
        void SetActive(bool state);

        /// <summary>
        /// Clear all displayed options in the Menu Dialog.
        /// </summary>
        void Clear();

        /// <summary>
        /// Hides any currently displayed Say Dialog.
        /// </summary>
        void HideSayDialog();

        /// <summary>
        /// Adds the option to the list of displayed options. Calls a Block when selected.
        /// Will cause the Menu dialog to become visible if it is not already visible.
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        /// <param name="text">The option text to display on the button.</param>
        /// <param name="interactable">If false, the option is displayed but is not selectable.</param>
        /// <param name="targetBlock">Block to execute when the option is selected.</param>
        bool AddOption(string text, bool interactable, Block targetBlock);

        /// <summary>
        /// Adds the option to the list of displayed options, calls a Lua function when selected.
        /// Will cause the Menu dialog to become visible if it is not already visible.
        /// </summary>
        /// <returns><c>true</c>, if the option was added successfully.</returns>
        bool AddOption(string text, bool interactable, ILuaEnvironment luaEnv, Closure callBack);

        /// <summary>
        /// Show a timer during which the player can select an option. Calls a Block when the timer expires.
        /// </summary>
        /// <param name="duration">The duration during which the player can select an option.</param>
        /// <param name="targetBlock">Block to execute if the player does not select an option in time.</param>
        void ShowTimer(float duration, Block targetBlock);

        /// <summary>
        /// Show a timer during which the player can select an option. Calls a Lua function when the timer expires.
        /// </summary>
        IEnumerator ShowTimer(float duration, ILuaEnvironment luaEnv, Closure callBack);

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