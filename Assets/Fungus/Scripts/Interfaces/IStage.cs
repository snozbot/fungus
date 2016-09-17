// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Define a set of screen positions where character sprites can be displayed.
    /// </summary>
    public interface IStage 
    {
        /// <summary>
        /// Canvas object containing the stage positions.
        /// </summary>
        Canvas PortraitCanvas { get; }

        /// <summary>
        /// Dim portraits when a character is not speaking.
        /// </summary>
        bool DimPortraits { get; set; }

        /// <summary>
        /// Duration for fading character portraits in / out.
        /// </summary>
        float FadeDuration { get; set; }

        /// <summary>
        /// Duration for moving characters to a new position.
        /// </summary>
        float MoveDuration { get; set; }

        /// <summary>
        /// Ease type for the fade tween.
        /// </summary>
        LeanTweenType FadeEaseType { get; }

        /// <summary>
        /// Constant offset to apply to portrait position.
        /// </summary>
        Vector2 ShiftOffset { get; }

        /// <summary>
        /// The position object where characters appear by default.
        /// </summary>
        Image DefaultPosition { get; }

        /// <summary>
        /// List of stage position rect transforms in the stage.
        /// </summary>
        List<RectTransform> Positions { get; }

        /// <summary>
        /// List of currently active characters on the stage.
        /// </summary>
        List<Character> CharactersOnStage { get; }

        /// <summary>
        /// Searches the stage's named positions
        /// If none matches the string provided, give a warning and return a new RectTransform
        /// </summary>
        RectTransform GetPosition(string positionString);
    }
}