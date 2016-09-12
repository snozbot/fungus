using UnityEngine;
using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Types of display operations supported by portraits
    /// </summary>
    public enum DisplayType
    {
        None,
        Show,
        Hide,
        Replace,
        MoveToFront
    }

    /// <summary>
    /// Directions that character portraits can face.
    /// </summary>
    public enum FacingDirection
    {
        None,
        Left,
        Right
    }

    /// <summary>
    /// Offset direction for position.
    /// </summary>
    public enum PositionOffset
    {
        None,
        OffsetLeft,
        OffsetRight
    }

    /// <summary>
    /// Controls the Portrait sprites on stage
    /// </summary>
    public interface IPortraitController
    {
        /// <summary>
        /// Using all portrait options available, run any portrait command.
        /// </summary>
        /// <param name="options">Portrait Options</param>
        /// <param name="onComplete">The function that will run once the portrait command finishes</param>
        void RunPortraitCommand(PortraitOptions options, System.Action onComplete);

        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        void MoveToFront(Character character);

        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        void MoveToFront(PortraitOptions options);

        /// <summary>
        /// Shows character at a named position in the stage
        /// </summary>
        /// <param name="character"></param>
        /// <param name="position">Named position on stage</param>
        void Show(Character character, string position);

        /// <summary>
        /// Shows character moving from a position to a position
        /// </summary>
        /// <param name="character"></param>
        /// <param name="portrait"></param>
        /// <param name="fromPosition">Where the character will appear</param>
        /// <param name="toPosition">Where the character will move to</param>
        void Show(Character character, string portrait, string fromPosition, string toPosition);

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.show{character=jill, portrait="happy", fromPosition="right", toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        void Show(Table optionsTable);

        /// <summary>
        /// Show portrait with the supplied portrait options
        /// </summary>
        /// <param name="options"></param>
        void Show(PortraitOptions options);

        /// <summary>
        /// Simple show command that shows the character with an available named portrait
        /// </summary>
        /// <param name="character">Character to show</param>
        /// <param name="portrait">Named portrait to show for the character, i.e. "angry", "happy", etc</param>
        void ShowPortrait(Character character, string portrait);

        /// <summary>
        /// Simple character hide command
        /// </summary>
        /// <param name="character">Character to hide</param>
        void Hide(Character character);

        /// <summary>
        /// Move the character to a position then hide it
        /// </summary>
        /// <param name="character">Character to hide</param>
        /// <param name="toPosition">Where the character will disapear to</param>
        void Hide(Character character, string toPosition);

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.hide{character=jill, toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        void Hide(Table optionsTable);

        /// <summary>
        /// Hide portrait with provided options
        /// </summary>
        void Hide(PortraitOptions options);
    }
}